export const PRODUCT_CODE_LENGTH = 16;
export const PRODUCT_CODE_PATTERN = /^[A-Z0-9]{4}(-[A-Z0-9]{4}){3}$/;

 
export function formatProductCodeInput(value: string): string {
  const raw = value
    .toUpperCase()
    .replace(/[^A-Z0-9]/g, '')
    .slice(0, PRODUCT_CODE_LENGTH);

  return raw.match(/.{1,4}/g)?.join('-') ?? '';
}

export function isValidProductCode(value: string): boolean {
  return PRODUCT_CODE_PATTERN.test(value);
}
