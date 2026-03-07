import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { InvoiceService } from '../../../services/invoice.service';
import { CustomerService } from '../../../services/customer.service';
import { OrderService } from '../../../services/order.service';
import { InvoiceDto, CustomerDto, OrderDto } from '../../../models/models';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.css'],
})
export class InvoicesComponent implements OnInit {
  invoices: InvoiceDto[] = [];
  customers: CustomerDto[] = [];
  orders: OrderDto[] = [];
  totalRecords = 0;
  totalPages = 1;
  currentPage = 1;
  pageSize = 10;
  customerName = '';
  dateFrom = '';
  dateTo = '';
  loading = false;
  saving = false;
  
  isAdmin = false;
  showAddModal = false;
  showEditModal = false;
  showDeleteModal = false;
  selectedInvoice: InvoiceDto | null = null;
  
  invoiceForm: any = {
    invoiceId: 0,
    customerId: 0,
    orderId: 0,
    invoiceDate: '',
    isCreditNote: false,
    comments: '',
    deliveryInstructions: '',
    totalDryItems: 0,
    totalChillerItems: 0
  };

  private searchTimeout: any;

  constructor(
    private invoiceService: InvoiceService,
    private customerService: CustomerService,
    private orderService: OrderService,
    private authService: AuthService
  ) {
    this.isAdmin = this.authService.isAdmin();
  }

  ngOnInit(): void {
    this.loadCustomers();
    this.loadOrders();
    this.loadInvoices();
  }

  loadCustomers(): void {
    this.customerService.getCustomers(1, 1000).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.customers = response.data;
        }
      },
      error: () => {}
    });
  }

  loadOrders(): void {
    this.orderService.getOrders(1, 1000).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.orders = response.data;
        }
      },
      error: () => {}
    });
  }

  loadInvoices(): void {
    this.loading = true;
    this.invoiceService.getInvoices(
      this.currentPage, 
      this.pageSize, 
      this.customerName || undefined,
      this.dateFrom || undefined,
      this.dateTo || undefined
    ).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.invoices = response.data;
          this.totalRecords = response.totalRecords;
          this.totalPages = response.totalPages;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage = 1;
      this.loadInvoices();
    }, 300);
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadInvoices();
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadInvoices();
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadInvoices();
    }
  }

  editInvoice(invoice: InvoiceDto): void {
    this.selectedInvoice = invoice;
    this.invoiceForm = {
      invoiceId: invoice.invoiceId,
      customerId: invoice.customerId,
      orderId: invoice.orderId || 0,
      invoiceDate: invoice.invoiceDate ? invoice.invoiceDate.split('T')[0] : '',
      isCreditNote: invoice.isCreditNote || false,
      comments: invoice.comments || '',
      deliveryInstructions: invoice.deliveryInstructions || '',
      totalDryItems: invoice.totalDryItems || 0,
      totalChillerItems: invoice.totalChillerItems || 0
    };
    this.showEditModal = true;
  }

  deleteInvoice(invoice: InvoiceDto): void {
    this.selectedInvoice = invoice;
    this.showDeleteModal = true;
  }

  saveInvoice(): void {
    if (!this.invoiceForm.customerId || !this.invoiceForm.invoiceDate) {
      return;
    }

    this.saving = true;

    if (this.showEditModal && this.selectedInvoice) {
      this.invoiceService.updateInvoice(this.invoiceForm.invoiceId, this.invoiceForm).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.loadInvoices();
            this.closeModals();
          }
          this.saving = false;
        },
        error: () => {
          this.saving = false;
        }
      });
    } else {
      this.invoiceService.createInvoice(this.invoiceForm).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.loadInvoices();
            this.closeModals();
          }
          this.saving = false;
        },
        error: () => {
          this.saving = false;
        }
      });
    }
  }

  confirmDelete(): void {
    if (!this.selectedInvoice) return;

    this.saving = true;
    this.invoiceService.deleteInvoice(this.selectedInvoice.invoiceId).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.loadInvoices();
          this.showDeleteModal = false;
        }
        this.saving = false;
      },
      error: () => {
        this.saving = false;
      }
    });
  }

  closeModals(): void {
    this.showAddModal = false;
    this.showEditModal = false;
    this.selectedInvoice = null;
    this.invoiceForm = {
      invoiceId: 0,
      customerId: 0,
      orderId: 0,
      invoiceDate: '',
      isCreditNote: false,
      comments: '',
      deliveryInstructions: '',
      totalDryItems: 0,
      totalChillerItems: 0
    };
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 2
    }).format(value);
  }
}
