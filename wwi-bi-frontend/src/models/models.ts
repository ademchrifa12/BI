// Authentication Models
export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface AuthResponse {
  success: boolean;
  token?: string;
  expiration?: string;
  message?: string;
  user?: User;
}

export interface User {
  userId: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  isActive: boolean;
  lastLoginAt?: string;
}

// API Response Wrapper
export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors: string[];
}

// Customer Models
export interface Customer {
  customerId: number;
  customerName: string;
  category?: string;
  city?: string;
  phoneNumber?: string;
  emailAddress?: string;
  postalAddress?: string;
  websiteUrl?: string;
  deliveryAddressLine1?: string;
  deliveryAddressLine2?: string;
  deliveryPostalCode?: string;
  creditLimit?: number;
  isOnCreditHold: boolean;
  accountOpenedDate: string;
  orderCount: number;
}

export interface CustomerDetail extends Customer {
  faxNumber?: string;
  standardDiscountPercentage: number;
  paymentDays: number;
  isStatementSent: boolean;
  totalOrders?: number;
  totalSales?: number;
  recentOrders: OrderSummary[];
  recentInvoices: InvoiceSummary[];
}

export interface CustomerCreate {
  customerName: string;
  phoneNumber?: string;
  websiteUrl?: string;
  deliveryAddressLine1?: string;
  deliveryAddressLine2?: string;
  deliveryPostalCode?: string;
  creditLimit?: number;
}

export interface CustomerUpdate extends CustomerCreate {
  isOnCreditHold: boolean;
}

// Product Models
export interface Product {
  stockItemId: number;
  stockItemName: string;
  brand?: string;
  size?: string;
  color?: string;
  unitPrice: number;
  recommendedRetailPrice?: number;
  taxRate: number;
  isChillerStock: boolean;
  quantityPerOuter: number;
  quantityOnHand?: number;
}

export interface ProductDetail extends Product {
  barcode?: string;
  leadTimeDays: number;
  typicalWeightPerUnit: number;
  marketingComments?: string;
  totalSalesQuantity: number;
  totalSalesAmount: number;
}

// Order Models
export interface Order {
  orderId: number;
  customerId: number;
  customerName: string;
  salespersonName?: string;
  orderDate: string;
  expectedDeliveryDate: string;
  customerPurchaseOrderNumber?: string;
  isPickingCompleted: boolean;
  isFinalized?: boolean;
  totalItems?: number;
  pickedByPersonName?: string;
  pickingCompletedWhen?: string;
  comments?: string;
  deliveryInstructions?: string;
  isUndersupplyBackordered?: boolean;
}

export interface OrderSummary {
  orderId: number;
  orderDate: string;
  expectedDeliveryDate: string;
  isPickingCompleted: boolean;
  isFinalized?: boolean;
}

export interface OrderDetail extends Order {
  comments?: string;
  deliveryInstructions?: string;
  isUndersupplyBackordered: boolean;
}

// Invoice Models
export interface Invoice {
  invoiceId: number;
  customerId: number;
  customerName: string;
  billToCustomerName?: string;
  invoiceDate: string;
  isCreditNote: boolean;
  isConfirmed?: boolean;
  totalLines: number;
  totalAmount: number;
  totalTax: number;
  orderId?: number;
  comments?: string;
  deliveryInstructions?: string;
  totalDryItems?: number;
  totalChillerItems?: number;
}

export interface InvoiceSummary {
  invoiceId: number;
  invoiceDate: string;
  totalAmount: number;
  isCreditNote: boolean;
}

export interface InvoiceDetail extends Invoice {
  orderId?: number;
  salespersonName?: string;
  deliveryMethod?: string;
  comments?: string;
  deliveryInstructions?: string;
  totalDryItems: number;
  totalChillerItems: number;
  lines: InvoiceLine[];
  invoiceLines?: InvoiceLine[];
}

export interface InvoiceLine {
  invoiceLineId: number;
  stockItemId: number;
  stockItemName: string;
  description: string;
  quantity: number;
  unitPrice: number;
  taxRate: number;
  taxAmount: number;
  extendedPrice: number;
  lineProfit: number;
}

// Analytics Models
export interface SalesByPeriod {
  year: number;
  month: number;
  monthName: string;
  period?: string;
  totalSales: number;
  totalTax: number;
  totalProfit: number;
  transactionCount: number;
}

export interface SalesByProduct {
  stockItemId: number;
  stockItemName: string;
  productName?: string;
  totalQuantity: number;
  totalSales: number;
  totalProfit: number;
  transactionCount: number;
}

export interface SalesByCustomer {
  customerId: number;
  customerName: string;
  totalOrders: number;
  totalInvoices: number;
  totalSales: number;
  totalProfit: number;
}

export interface Kpi {
  totalRevenue: number;
  totalSales: number;
  totalOrders: number;
  totalCustomers: number;
  totalProducts: number;
  totalInvoices: number;
  averageOrderValue: number;
  totalProfit: number;
  profitMargin: number;
  dateRange?: DateRangeInfo;
}

export interface DateRangeInfo {
  earliestDate?: string;
  latestDate?: string;
}

export interface DashboardData {
  salesByPeriod: SalesByPeriod[];
  salesByProduct: SalesByProduct[];
  salesByCustomer: SalesByCustomer[];
  kpis: Kpi;
}

// Paginated Response
export interface PaginatedResponse<T> {
  success: boolean;
  data: T[];
  totalRecords: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
  message?: string;
}

// Alias exports for component compatibility (all properties now in base interfaces)
export type CustomerDto = Customer;
export type CustomerDetailDto = CustomerDetail;
export type ProductDto = Product;
export type OrderDto = Order;
export type InvoiceDto = Invoice;
export type InvoiceDetailDto = InvoiceDetail;
export type KpiDto = Kpi;
export type SalesByPeriodDto = SalesByPeriod;
export type SalesByProductDto = SalesByProduct;
export type SalesByCustomerDto = SalesByCustomer;

// ── Data Warehouse Dashboard Models ──────────────────────────────────────────

export interface DwKpi {
  totalRevenue: number;
  totalProfit: number;
  totalTax: number;
  totalTransactions: number;
  uniqueClients: number;
  uniqueProducts: number;
  averageTransactionValue: number;
  profitMarginPercent: number;
}

export interface DwSalesByProduct {
  produitSK: number;
  productName: string;
  brand?: string;
  stockGroup?: string;
  totalQuantity: number;
  totalRevenue: number;
  totalProfit: number;
}

export interface DwSalesByClient {
  clientSK: number;
  customerName: string;
  category?: string;
  city?: string;
  totalTransactions: number;
  totalQuantity: number;
  totalRevenue: number;
  totalProfit: number;
}

export interface DwSalesByEmployee {
  employeSK: number;
  fullName: string;
  totalTransactions: number;
  totalQuantity: number;
  totalRevenue: number;
  totalProfit: number;
}
