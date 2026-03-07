import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CustomerService } from '../../../services/customer.service';
import { CustomerDto } from '../../../models/models';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.css'],
})
export class CustomersComponent implements OnInit {
  customers: CustomerDto[] = [];
  totalRecords = 0;
  totalPages = 1;
  currentPage = 1;
  pageSize = 10;
  searchTerm = '';
  loading = false;
  saving = false;

  showAddModal = false;
  showEditModal = false;
  showDeleteModal = false;
  selectedCustomer: CustomerDto | null = null;

  customerForm = {
    customerId: 0,
    customerName: '',
    category: '',
    city: '',
    phoneNumber: '',
    emailAddress: '',
    postalAddress: ''
  } as any;

  isAdmin = false;

  private searchTimeout: any;

  constructor(
    private customerService: CustomerService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.hasRole('Admin');
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.loading = true;
    this.customerService.getCustomers(this.currentPage, this.pageSize, this.searchTerm).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.customers = response.data;
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
      this.loadCustomers();
    }, 300);
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadCustomers();
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadCustomers();
    }
  }

  editCustomer(customer: CustomerDto): void {
    this.selectedCustomer = customer;
    this.customerForm = { ...customer } as any;
    this.showEditModal = true;
  }

  deleteCustomer(customer: CustomerDto): void {
    this.selectedCustomer = customer;
    this.showDeleteModal = true;
  }

  saveCustomer(): void {
    if (!this.customerForm.customerName.trim()) {
      return;
    }

    this.saving = true;

    if (this.showEditModal && this.selectedCustomer) {
      this.customerService.updateCustomer(this.customerForm.customerId, this.customerForm).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.loadCustomers();
            this.closeModals();
          }
          this.saving = false;
        },
        error: () => {
          this.saving = false;
        }
      });
    } else {
      this.customerService.createCustomer(this.customerForm).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.loadCustomers();
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
    if (!this.selectedCustomer) return;

    this.saving = true;
    this.customerService.deleteCustomer(this.selectedCustomer.customerId).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.loadCustomers();
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
    this.selectedCustomer = null;
    this.customerForm = {
      customerId: 0,
      customerName: '',
      category: '',
      city: '',
      phoneNumber: '',
      emailAddress: '',
      postalAddress: ''
    } as any;
  }
}
