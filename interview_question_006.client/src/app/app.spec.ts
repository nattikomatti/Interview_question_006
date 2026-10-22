import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { App } from './app';
import { ConfirmDialog } from './shared/confirm-dialog/confirm-dialog';

describe('App', () => {
  let fixture: ComponentFixture<App>;
  let httpMock: HttpTestingController;

  const products = [
    { id: 7, productCode: 'ABCD-1234-EFGH-5678', createdAt: '2026-10-21T00:00:00Z' },
    { id: 9, productCode: 'ZZZZ-0000-YYYY-1111', createdAt: '2026-10-21T00:00:00Z' },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [App, ConfirmDialog],
      imports: [FormsModule],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    httpMock.expectOne('/api/products').flush(products);
    await fixture.whenStable();
  });

  afterEach(() => httpMock.verify());

  function element(): HTMLElement {
    return fixture.nativeElement as HTMLElement;
  }

  function button(text: string): HTMLButtonElement {
    return Array.from(element().querySelectorAll('button')).find(
      (b) => b.textContent?.trim() === text,
    )!;
  }

  it('renders rows numbered from 1 with barcode images', () => {
    const rows = element().querySelectorAll('tbody tr');

    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('ABCD-1234-EFGH-5678');
    expect(rows[1].querySelector('td')?.textContent?.trim()).toBe('2');
    expect(rows[0].querySelector('img')?.getAttribute('src')).toBe('/api/products/7/barcode');
  });

  it('formats input and posts the product code', async () => {
    const input = element().querySelector('input')!;
    input.value = 'abcd1234efgh5678';
    input.dispatchEvent(new Event('input'));
    expect(input.value).toBe('ABCD-1234-EFGH-5678');

    button('ADD').click();
    const req = httpMock.expectOne({ method: 'POST', url: '/api/products' });
    expect(req.request.body).toEqual({ productCode: 'ABCD-1234-EFGH-5678' });
    req.flush(products[0]);

    httpMock.expectOne('/api/products').flush(products);
  });

  it('does not post an incomplete code', async () => {
    const input = element().querySelector('input')!;
    input.value = 'ABCD12';
    input.dispatchEvent(new Event('input'));

    button('ADD').click();
    await fixture.whenStable();

    httpMock.expectNone({ method: 'POST' });
    expect(element().querySelector('.error')).not.toBeNull();
  });

  it('asks for confirmation before deleting', async () => {
    button('ลบ').click();
    await fixture.whenStable();

    expect(element().querySelector('app-confirm-dialog')?.textContent).toContain(
      'ต้องการลบข้อมูล รหัสสินค้า ABCD-1234-EFGH-5678 หรือไม่ ?',
    );
    httpMock.expectNone({ method: 'DELETE' });

    button('ตกลง').click();
    httpMock.expectOne({ method: 'DELETE', url: '/api/products/7' }).flush(null);
    httpMock.expectOne('/api/products').flush([products[1]]);
  });

  it('cancelling the confirmation keeps the product', async () => {
    button('ลบ').click();
    await fixture.whenStable();

    button('ยกเลิก').click();
    await fixture.whenStable();

    httpMock.expectNone({ method: 'DELETE' });
    expect(element().querySelector('app-confirm-dialog')).toBeNull();
  });
});
