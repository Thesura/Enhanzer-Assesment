import { FormControl } from '@angular/forms';
import { ValidatorUtil } from './validator.util';

describe('ValidatorUtil', () => {
  it('trims input and removes control characters', () => {
    expect(ValidatorUtil.sanitizeInput('  user\u0000@example.com \n')).toBe('user@example.com');
  });

  it('validates required and email inputs', () => {
    expect(ValidatorUtil.required()(new FormControl('   '))).toEqual({ required: true });
    expect(ValidatorUtil.email()(new FormControl('user@example.com'))).toBeNull();
    expect(ValidatorUtil.email()(new FormControl('not-an-email'))).toEqual({ email: true });
  });
});
