import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { OrderService } from '../../../services/order.service';
import { CustomerService } from '../../../services/customer.service';
import { OrderDto, CustomerDto } from '../../../models/models';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css'],
})
export class OrdersComponent implements OnInit {
  orders: OrderDto[] = [];
  customers: CustomerDto[] = [];
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
  selectedOrder: OrderDto | null = null;
  
  orderForm: any = {
    orderId: 0,
    customerId: 0,
    orderDate: '',
    expectedDeliveryDate: '',
    customerPurchaseOrderNumber: '',
    comments: '',
    deliveryInstructions: '',
    isUndersupplyBackordered: false,
    isPickingCompleted: false
  };

  private searchTimeout: any;

  constructor(
    private orderService: OrderService,
    private customerService: CustomerService,
    private authService: AuthService
  ) {
    this.isAdmin = this.authService.isAdmin();
  }

  ngOnInit(): void {
    this.loadCustomers();
    this.loadOrders();
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
    this.loading = true;
    this.orderService.getOrders(
      this.currentPage, 
      this.pageSize, 
      this.customerName || undefined,
      this.dateFrom || undefined,
      this.dateTo || undefined
    ).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.orders = response.data;
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
      this.loadOrders();
    }, 300);
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadOrders();
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadOrders();
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadOrders();
    }
  }

  editOrder(order: OrderDto): void {
    this.selectedOrder = order;
    this.orderForm = {
      orderId: order.orderId,
      customerId: order.customerId,
      orderDate: order.orderDate ? order.orderDate.split('T')[0] : '',
      expectedDeliveryDate: order.expectedDeliveryDate ? order.expectedDeliveryDate.split('T')[0] : '',
      customerPurchaseOrderNumber: order.customerPurchaseOrderNumber || '',
      comments: order.comments || '',
      deliveryInstructions: order.deliveryInstructions || '',
      isUndersupplyBackordered: order.isUndersupplyBackordered || false,
      isPickingCompleted: order.isPickingCompleted || false
    };
    this.showEditModal = true;
  }

  deleteOrder(order: OrderDto): void {
    this.selectedOrder = order;
    this.showDeleteModal = true;
  }

  saveOrder(): void {
    if (!this.orderForm.customerId || !this.orderForm.orderDate) {
      return;
    }

    this.saving = true;

    if (this.showEditModal && this.selectedOrder) {
      this.orderService.updateOrder(this.orderForm.orderId, this.orderForm).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.loadOrders();
            this.closeModals();
          }
          this.saving = false;
        },
        error: () => {
          this.saving = false;
        }
      });
    } else {
      this.orderService.createOrder(this.orderForm).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.loadOrders();
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
    if (!this.selectedOrder) return;

    this.saving = true;
    this.orderService.deleteOrder(this.selectedOrder.orderId).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.loadOrders();
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
    this.selectedOrder = null;
    this.orderForm = {
      orderId: 0,
      customerId: 0,
      orderDate: '',
      expectedDeliveryDate: '',
      customerPurchaseOrderNumber: '',
      comments: '',
      deliveryInstructions: '',
      isUndersupplyBackordered: false,
      isPickingCompleted: false
    };
  }
}
