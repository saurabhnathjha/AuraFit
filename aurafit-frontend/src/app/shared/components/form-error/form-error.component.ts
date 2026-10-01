// form-error.component.ts
import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-form-error',
  template: `
    <div *ngIf="control && control.invalid && (control.dirty || control.touched)" class="error">
      <small *ngIf="control.errors?.required">This field is required.</small>
      <small *ngIf="control.errors?.minlength">Minimum length not met.</small>
      <small *ngIf="control.errors?.email">Invalid email format.</small>
      <!-- Add more errors as needed -->
    </div>
  `,
  styles: [`
    .error { color: #e53935; font-size: 0.8rem; }
  `],
})
export class FormErrorComponent {
  @Input() control!: AbstractControl | null;
}
