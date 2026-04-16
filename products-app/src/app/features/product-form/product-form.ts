import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ProductService } from '../../../app/core/services/product.service';
import { Product } from '../../../app/shared/models/product.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss'
})
export class ProductFormComponent implements OnInit {
  @Input() product: Product | null = null;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form!: FormGroup;
  loading = false;

  get isEdit() { return !!this.product; }

  constructor(private fb: FormBuilder, private productService: ProductService) {}

  ngOnInit() {
    this.form = this.fb.group({
      name: [this.product?.name ?? '', [Validators.required, Validators.maxLength(100)]],
      description: [this.product?.description ?? '', Validators.required],
      price: [this.product?.price ?? null, [Validators.required, Validators.min(0)]],
      stock: [this.product?.stock ?? null, [Validators.required, Validators.min(0)]]
    });
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const dto = this.form.value;

    if (this.isEdit) {
      this.productService.update(this.product!.id, dto).subscribe({
        next: () => this.saved.emit(),
        error: () => (this.loading = false)
      });
      return;
    }

    this.productService.create(dto).subscribe({
      next: () => this.saved.emit(),
      error: () => (this.loading = false)
    });
  }

  hasError(field: string) {
    const control = this.form.get(field);
    return control?.invalid && control?.touched;
  }
}