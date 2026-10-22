import { formatProductCodeInput, isValidProductCode } from './product-code';

describe('formatProductCodeInput', () => {
  it('uppercases and inserts dashes', () => {
    expect(formatProductCodeInput('abcd1234efgh5678')).toBe('ABCD-1234-EFGH-5678');
  });

  it('drops characters outside A-Z and 0-9', () => {
    expect(formatProductCodeInput('AB@C D-12_34')).toBe('ABCD-1234');
  });

  it('limits to 16 characters', () => {
    expect(formatProductCodeInput('ABCD1234EFGH5678XYZ')).toBe('ABCD-1234-EFGH-5678');
  });

  it('keeps partial input formatted', () => {
    expect(formatProductCodeInput('ABCDE')).toBe('ABCD-E');
    expect(formatProductCodeInput('')).toBe('');
  });
});

describe('isValidProductCode', () => {
  it('accepts the full format', () => {
    expect(isValidProductCode('ABCD-1234-EFGH-5678')).toBe(true);
  });

  it.each(['ABCD-1234-EFGH-567', 'abcd-1234-efgh-5678', 'ABCD1234EFGH5678', 'ABCD-1234-EFGH-567@'])(
    'rejects %s',
    (value) => {
      expect(isValidProductCode(value)).toBe(false);
    },
  );
});
