import { AbstractControl, ValidatorFn, ValidationErrors } from '@angular/forms';

// Validateurs personnalisés pour les formulaires
export class FormValidators {
  // Validateur pour email
  static email(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailRegex.test(control.value) ? null : { email: true };
  }

  // Validateur pour mot de passe fort
  static strongPassword(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const errors: ValidationErrors = {};
    
    if (control.value.length < 8) {
      errors['minLength'] = true;
    }
    
    if (!/(?=.*[a-z])/.test(control.value)) {
      errors['lowercase'] = true;
    }
    
    if (!/(?=.*[A-Z])/.test(control.value)) {
      errors['uppercase'] = true;
    }
    
    if (!/(?=.*\d)/.test(control.value)) {
      errors['digit'] = true;
    }
    
    if (!/(?=.*[@$!%*?&])/.test(control.value)) {
      errors['specialChar'] = true;
    }
    
    return Object.keys(errors).length ? errors : null;
  }

  // Validateur pour confirmation de mot de passe
  static passwordMatch(passwordControlName: string, confirmPasswordControlName: string): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
      const password = formGroup.get(passwordControlName)?.value;
      const confirmPassword = formGroup.get(confirmPasswordControlName)?.value;
      
      if (password !== confirmPassword) {
        formGroup.get(confirmPasswordControlName)?.setErrors({ passwordMismatch: true });
        return { passwordMismatch: true };
      }
      
      return null;
    };
  }

  // Validateur pour ISBN
  static isbn(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const isbn = control.value.replace(/[- ]/g, '');
    
    // ISBN-10 ou ISBN-13
    if (isbn.length === 10) {
      return FormValidators.validateIsbn10(isbn) ? null : { isbn: true };
    } else if (isbn.length === 13) {
      return FormValidators.validateIsbn13(isbn) ? null : { isbn: true };
    }
    
    return { isbn: true };
  }

  private static validateIsbn10(isbn: string): boolean {
    let sum = 0;
    for (let i = 0; i < 9; i++) {
      sum += parseInt(isbn[i]) * (10 - i);
    }
    
    const lastChar = isbn[9];
    if (lastChar === 'X' || lastChar === 'x') {
      sum += 10;
    } else {
      sum += parseInt(lastChar);
    }
    
    return sum % 11 === 0;
  }

  private static validateIsbn13(isbn: string): boolean {
    let sum = 0;
    for (let i = 0; i < 12; i++) {
      sum += parseInt(isbn[i]) * (i % 2 === 0 ? 1 : 3);
    }
    
    const checkDigit = parseInt(isbn[12]);
    const calculatedCheckDigit = (10 - (sum % 10)) % 10;
    
    return checkDigit === calculatedCheckDigit;
  }

  // Validateur pour date future
  static futureDate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const selectedDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    return selectedDate >= today ? null : { futureDate: true };
  }

  // Validateur pour date passée
  static pastDate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const selectedDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    return selectedDate <= today ? null : { pastDate: true };
  }

  // Validateur pour nombre positif
  static positiveNumber(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const value = parseFloat(control.value);
    return value > 0 ? null : { positiveNumber: true };
  }

  // Validateur pour nombre entier positif
  static positiveInteger(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const value = parseInt(control.value);
    return Number.isInteger(value) && value > 0 ? null : { positiveInteger: true };
  }

  // Validateur pour URL
  static url(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    try {
      new URL(control.value);
      return null;
    } catch {
      return { url: true };
    }
  }

  // Validateur pour téléphone français
  static frenchPhone(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const phoneRegex = /^(?:(?:\+|00)33|0)\s*[1-9](?:[\s.-]*\d{2}){4}$/;
    return phoneRegex.test(control.value) ? null : { frenchPhone: true };
  }

  // Validateur pour code postal français
  static frenchPostalCode(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    
    const postalCodeRegex = /^\d{5}$/;
    return postalCodeRegex.test(control.value) ? null : { frenchPostalCode: true };
  }

  // Validateur pour valeur non vide (trim)
  static notEmpty(control: AbstractControl): ValidationErrors | null {
    if (!control.value || control.value.trim() === '') {
      return { notEmpty: true };
    }
    
    return null;
  }

  // Validateur pour longueur minimale
  static minLength(min: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      
      return control.value.length >= min ? null : { minLength: { requiredLength: min, actualLength: control.value.length } };
    };
  }

  // Validateur pour longueur maximale
  static maxLength(max: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      
      return control.value.length <= max ? null : { maxLength: { requiredLength: max, actualLength: control.value.length } };
    };
  }

  // Validateur pour valeur dans une liste
  static inList(allowedValues: any[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      
      return allowedValues.includes(control.value) ? null : { inList: true };
    };
  }
}