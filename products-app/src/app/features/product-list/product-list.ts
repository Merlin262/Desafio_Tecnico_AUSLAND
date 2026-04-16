import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ProductService } from '../../../app/core/services/product.service';
import { Product } from '../../../app/shared/models/product.model';
import { ProductFormComponent } from '../product-form/product-form';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, DatePipe, ProductFormComponent],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class ProductListComponent implements OnInit {
  products = signal<Product[]>([]);
  loading = signal(false);
  showForm = signal(false);
  selectedProduct = signal<Product | null>(null);
  toast = signal<{ message: string; type: 'success' | 'error' } | null>(null);

  pageNumber = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.productService.getAll(this.pageNumber(), this.pageSize()).subscribe({
      next: (data) => {
        this.products.set(data.items);
        this.totalCount.set(data.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.showToast('Erro ao carregar produtos.', 'error');
        this.loading.set(false);
      }
    });
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.load();
  }

  openCreate() {
    this.selectedProduct.set(null);
    this.showForm.set(true);
  }

  openEdit(product: Product) {
    this.selectedProduct.set(product);
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.selectedProduct.set(null);
  }

  onSaved() {
    this.closeForm();
    this.load();
    this.showToast('Produto salvo com sucesso!', 'success');
  }

  delete(id: number) {
    if (!confirm('Deseja excluir este produto?')) return;

    this.productService.delete(id).subscribe({
      next: () => {
        this.load();
        this.showToast('Produto excluído.', 'success');
      },
      error: () => this.showToast('Erro ao excluir produto.', 'error')
    });
  }

  private showToast(message: string, type: 'success' | 'error') {
    this.toast.set({ message, type });
    setTimeout(() => this.toast.set(null), 3000);
  }
}