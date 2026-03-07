import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { ProductDto } from '../../../models/models';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css'],
})
export class ProductsComponent implements OnInit {
  products: ProductDto[] = [];
  totalRecords = 0;
  totalPages = 1;
  currentPage = 1;
  pageSize = 12;
  searchTerm = '';
  loading = false;
  saving = false;
  
  isAdmin = false;
  showAddModal = false;
  showEditModal = false;
  showDeleteModal = false;
  selectedProduct: ProductDto | null = null;
  
  productForm: any = {
    stockItemId: 0,
    stockItemName: '',
    brand: '',
    size: '',
    unitPrice: 0,
    recommendedRetailPrice: 0,
    taxRate: 0,
    quantityPerOuter: 0,
    barcode: '',
    leadTimeDays: 0,
    isChillerStock: false
  };

  private searchTimeout: any;

  constructor(
    private productService: ProductService,
    private authService: AuthService
  ) {
    this.isAdmin = this.authService.isAdmin();
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.productService.getProducts(this.currentPage, this.pageSize, this.searchTerm).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.products = response.data;
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
      this.loadProducts();
    }, 300);
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadProducts();
    }
  }

  editProduct(product: ProductDto): void {
    this.selectedProduct = product;
    this.productForm = { ...product };
    this.showEditModal = true;
  }

  deleteProduct(product: ProductDto): void {
    this.selectedProduct = product;
    this.showDeleteModal = true;
  }

  saveProduct(): void {
    if (!this.productForm.stockItemName.trim()) {
      return;
    }

    this.saving = true;

    if (this.showEditModal && this.selectedProduct) {
      this.productService.updateProduct(this.productForm.stockItemId, this.productForm).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.loadProducts();
            this.closeModals();
          }
          this.saving = false;
        },
        error: () => {
          this.saving = false;
        }
      });
    } else {
      this.productService.createProduct(this.productForm).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.loadProducts();
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
    if (!this.selectedProduct) return;

    this.saving = true;
    this.productService.deleteProduct(this.selectedProduct.stockItemId).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.loadProducts();
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
    this.selectedProduct = null;
    this.productForm = {
      stockItemId: 0,
      stockItemName: '',
      brand: '',
      size: '',
      unitPrice: 0,
      recommendedRetailPrice: 0,
      taxRate: 0,
      quantityPerOuter: 0,
      barcode: '',
      leadTimeDays: 0,
      isChillerStock: false
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
