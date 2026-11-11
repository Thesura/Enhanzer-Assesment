import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class ValidatorUtil {
  private static readonly emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  static sanitizeInput(value: string): string {
    return value.replace(/[\u0000-\u001F\u007F]/g, '').trim();
  }

  static required(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null =>
      typeof control.value === 'string' && control.value.trim().length > 0
        ? null
        : { required: true };
  }

  static email(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (typeof control.value !== 'string') {
        return null;
      }

      const value = this.sanitizeInput(control.value);
      return value.length === 0 || this.emailPattern.test(value)
        ? null
        : { email: true };
    };
  }
}
