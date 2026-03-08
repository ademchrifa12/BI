import { Component, OnInit, ElementRef, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { DwDashboardService } from '../../../services/dw-dashboard.service';
import {
  DwKpi,
  DwSalesByEmployee,
  DwSalesByProduct,
  DwSalesByClient
} from '../../../models/models';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('salesByEmployeeChart') salesByEmployeeCanvas!: ElementRef<HTMLCanvasElement>;
  @ViewChild('salesByProductChart') salesByProductCanvas!: ElementRef<HTMLCanvasElement>;
  @ViewChild('salesByCustomerChart') salesByCustomerCanvas!: ElementRef<HTMLCanvasElement>;

  kpis: DwKpi | null = null;
  salesByEmployee: DwSalesByEmployee[] = [];
  salesByProduct: DwSalesByProduct[] = [];
  salesByCustomer: DwSalesByClient[] = [];
  loading = true;

  private employeeChart: Chart | null = null;
  private productChart: Chart | null = null;
  private customerChart: Chart | null = null;

  constructor(private dwService: DwDashboardService) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  ngAfterViewInit(): void {
    // Charts will be initialized after data is loaded
  }

  ngOnDestroy(): void {
    this.destroyCharts();
  }

  loadDashboardData(): void {
    this.loading = true;

    // Load all data in parallel from the Data Warehouse
    Promise.all([
      this.dwService.getKpis().toPromise(),
      this.dwService.getSalesByEmployee().toPromise(),
      this.dwService.getTopProducts(10).toPromise(),
      this.dwService.getTopClients(10).toPromise()
    ]).then(([kpisResponse, employeeResponse, productResponse, customerResponse]) => {
      if (kpisResponse?.success && kpisResponse.data) {
        this.kpis = kpisResponse.data;
      }
      if (employeeResponse?.success && employeeResponse.data) {
        this.salesByEmployee = employeeResponse.data;
      }
      if (productResponse?.success && productResponse.data) {
        this.salesByProduct = productResponse.data;
      }
      if (customerResponse?.success && customerResponse.data) {
        this.salesByCustomer = customerResponse.data;
      }

      this.loading = false;
      
      // Initialize charts after a small delay to ensure DOM is ready
      setTimeout(() => this.initCharts(), 100);
    }).catch(error => {
      console.error('Error loading dashboard data:', error);
      this.loading = false;
    });
  }

  private initCharts(): void {
    this.destroyCharts();
    this.initSalesByEmployeeChart();
    this.initSalesByProductChart();
    this.initSalesByCustomerChart();
  }

  private destroyCharts(): void {
    if (this.employeeChart) {
      this.employeeChart.destroy();
      this.employeeChart = null;
    }
    if (this.productChart) {
      this.productChart.destroy();
      this.productChart = null;
    }
    if (this.customerChart) {
      this.customerChart.destroy();
      this.customerChart = null;
    }
  }

  private initSalesByEmployeeChart(): void {
    if (!this.salesByEmployeeCanvas?.nativeElement) return;

    const ctx = this.salesByEmployeeCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const labels = this.salesByEmployee.map(e => e.fullName);
    const data = this.salesByEmployee.map(e => e.totalRevenue);

    const colors = [
      '#1976d2', '#388e3c', '#f57c00', '#7b1fa2', '#c2185b',
      '#0097a7', '#5d4037', '#455a64', '#e64a19', '#512da8',
      '#827717', '#1b5e20', '#4a148c', '#880e4f', '#01579b',
      '#006064', '#e65100', '#bf360c', '#37474f', '#0d47a1'
    ];

    this.employeeChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Revenue',
          data,
          backgroundColor: colors.slice(0, data.length),
          borderRadius: 6,
          barThickness: 22
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: 'y',
        plugins: {
          legend: { display: false },
          tooltip: {
            callbacks: {
              label: (context) => `$${(context.raw as number).toLocaleString()}`
            }
          }
        },
        scales: {
          x: {
            beginAtZero: true,
            ticks: {
              callback: (value) => '$' + Number(value).toLocaleString()
            }
          }
        }
      }
    });
  }

  private initSalesByProductChart(): void {
    if (!this.salesByProductCanvas?.nativeElement) return;

    const ctx = this.salesByProductCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const labels = this.salesByProduct.map(d => {
      const name = d.productName || 'Unknown';
      return name.length > 20 ? name.substring(0, 20) + '...' : name;
    });
    const data = this.salesByProduct.map(d => d.totalRevenue);

    const colors = [
      '#1976d2', '#388e3c', '#f57c00', '#7b1fa2', '#c2185b',
      '#0097a7', '#5d4037', '#455a64', '#e64a19', '#512da8'
    ];

    this.productChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Sales',
          data,
          backgroundColor: colors.slice(0, data.length),
          borderRadius: 8,
          barThickness: 30
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: 'y',
        plugins: {
          legend: {
            display: false
          },
          tooltip: {
            callbacks: {
              label: (context) => `$${context.raw?.toLocaleString()}`
            }
          }
        },
        scales: {
          x: {
            beginAtZero: true,
            ticks: {
              callback: (value) => '$' + Number(value).toLocaleString()
            }
          }
        }
      }
    });
  }

  private initSalesByCustomerChart(): void {
    if (!this.salesByCustomerCanvas?.nativeElement) return;

    const ctx = this.salesByCustomerCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const labels = this.salesByCustomer.map(d =>
      d.customerName.length > 25 ? d.customerName.substring(0, 25) + '...' : d.customerName
    );
    const data = this.salesByCustomer.map(d => d.totalRevenue);

    const colors = [
      '#f44336', '#e91e63', '#9c27b0', '#673ab7', '#3f51b5',
      '#2196f3', '#03a9f4', '#00bcd4', '#009688', '#4caf50'
    ];

    this.customerChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels,
        datasets: [{
          data,
          backgroundColor: colors.slice(0, data.length),
          borderWidth: 0,
          hoverOffset: 4
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'right',
            labels: {
              boxWidth: 12,
              padding: 10,
              font: {
                size: 11
              }
            }
          },
          tooltip: {
            callbacks: {
              label: (context) => {
                const total = (context.dataset.data as number[]).reduce((a, b) => a + b, 0);
                const percentage = ((context.raw as number / total) * 100).toFixed(1);
                return `$${context.raw?.toLocaleString()} (${percentage}%)`;
              }
            }
          }
        }
      }
    });
  }

  formatCurrency(value: number): string {
    if (value >= 1000000) {
      return '$' + (value / 1000000).toFixed(1) + 'M';
    } else if (value >= 1000) {
      return '$' + (value / 1000).toFixed(1) + 'K';
    }
    return '$' + value.toFixed(0);
  }

  formatNumber(value: number): string {
    if (value >= 1000000) {
      return (value / 1000000).toFixed(1) + 'M';
    } else if (value >= 1000) {
      return (value / 1000).toFixed(1) + 'K';
    }
    return value.toLocaleString();
  }
}
