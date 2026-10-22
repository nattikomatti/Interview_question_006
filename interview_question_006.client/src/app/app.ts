import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';

import { formatProductCodeInput, isValidProductCode } from './products/product-code';
import { Product } from './products/product.model';
import { ProductService } from './products/product.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css',
})
export class App implements OnInit {
  private readonly productService = inject(ProductService);

  protected readonly title = 'IT 06-1';
  protected readonly products = signal<Product[]>([]);
  protected readonly productCode = signal('');
  protected readonly error = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected readonly pendingDelete = signal<Product | null>(null);

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getAll().subscribe({
      next: (products) => this.products.set(products),
      error: (err: HttpErrorResponse) => this.error.set(this.describeError(err)),
    });
  }

  onCodeInput(event: Event) {
    const input = event.target as HTMLInputElement;
    const formatted = formatProductCodeInput(input.value);
    input.value = formatted;
    this.productCode.set(formatted);
    this.error.set(null);
  }

  add() {
    const code = this.productCode();
    if (!isValidProductCode(code)) {
      this.error.set(
        'รหัสสินค้าต้องเป็นตัวเลขหรือตัวอักษรภาษาอังกฤษพิมพ์ใหญ่ 16 หลัก รูปแบบ XXXX-XXXX-XXXX-XXXX',
      );
      return;
    }

    this.saving.set(true);
    this.productService.create({ productCode: code }).subscribe({
      next: () => {
        this.saving.set(false);
        this.productCode.set('');
        this.loadProducts();
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.error.set(this.describeError(err));
      },
    });
  }

  askDelete(product: Product) {
    this.pendingDelete.set(product);
  }

  cancelDelete() {
    this.pendingDelete.set(null);
  }

  confirmDelete() {
    const product = this.pendingDelete();
    if (!product) {
      return;
    }

    this.pendingDelete.set(null);
    this.productService.delete(product.id).subscribe({
      next: () => this.loadProducts(),
      error: (err: HttpErrorResponse) => this.error.set(this.describeError(err)),
    });
  }

  barcodeUrl(product: Product): string {
    return this.productService.barcodeUrl(product.id);
  }

  deleteMessage(product: Product): string {
    return `ต้องการลบข้อมูล รหัสสินค้า ${product.productCode} หรือไม่ ?`;
  }

  private describeError(err: HttpErrorResponse): string {
    if (err.status === 409) {
      return 'รหัสสินค้านี้มีอยู่แล้ว';
    }
    if (err.status === 400) {
      return err.error?.detail ?? 'รหัสสินค้าไม่ถูกต้อง';
    }
    if (err.status === 404) {
      return 'ไม่พบข้อมูลสินค้า';
    }
    return 'ไม่สามารถเชื่อมต่อ server ได้ กรุณาลองใหม่';
  }
}
