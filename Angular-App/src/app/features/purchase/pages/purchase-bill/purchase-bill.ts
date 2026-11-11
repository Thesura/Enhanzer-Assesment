import { CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';
import { FetchRequest } from '../../../util/fetchRequest.util';


interface BillItem {
  id: string;
  name: string;
}

interface BatchOption {
  id: string;
  name: string;
}

interface BillLine {
  id: number;
  item: string;
  batch: string;
  standardCost: number;
  standardPrice: number;
  margin: number;
  quantity: number;
  discount: number;
  totalCost: number;
  totalPrice: number;
}

@Component({
  imports: [CurrencyPipe, ReactiveFormsModule],
  selector: 'app-purchase-bill-page',
  styleUrl: './purchase-bill.css',
  templateUrl: './purchase-bill.html',
})
export class PurchaseBillPage {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly batches: BatchOption[] = [];
  protected readonly loading = signal(true);

  async ngOnInit() {
    const fetchRequest = new FetchRequest(
      "http://localhost:5068/api/locations",
      {},
      'GET'
    );
    try {
      const response = await fetchRequest.execute<any>();
      if(response.status === 200) {
        const locations = response.data;
        console.log('Fetched locations:', locations);
        this.batches.push(...locations.map((location: any) => ({
          id: location.Location_ID,
          name: location.Location_Name,
        })));
      } else {
        this.statusMessage.set('Failed to fetch locations. Please try again later.');
      }
    } catch (error) {
      this.statusMessage.set('An error occurred while fetching locations. Please try again.');
      console.error('Error fetching locations:', error);
    } finally {
      this.loading.set(false);
    }
  }

  protected readonly username = this.authService.getUsername() ?? 'User';
  protected readonly items: BillItem[] = [
    { id: 'item-1', name: 'Mango'},
    { id: 'item-2', name: 'Apple'},
    { id: 'item-3', name: 'Banana'},
    { id: 'item-4', name: 'Orange'},
    { id: 'item-5', name: 'Grape'},
    { id: 'item-6', name: 'Kiwi'},
    { id: 'item-7', name: 'Strawberry'},
  ];
  
  protected readonly lines = signal<BillLine[]>([]);
  protected readonly statusMessage = signal('');
  protected readonly entryForm = new FormGroup({
    item: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    batch: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    standardCost: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(0),
    ]),
    standardPrice: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(0),
    ]),
    quantity: new FormControl<number | null>(1, [
      Validators.required,
      Validators.min(1),
    ]),
    discount: new FormControl<number | null>(0, [Validators.required, Validators.min(0)]),
  });

  private nextLineId = 1;

  protected get availableBatches(): string[] {
    return this.batches.map((batch) => batch.name);
  }

  protected get margin(): number {
    return (this.entryForm.controls.standardPrice.value ?? 0) -
      (this.entryForm.controls.standardCost.value ?? 0);
  }

  protected get totalCost(): number {
    let total = (this.entryForm.controls.standardCost.value ?? 0) * (this.entryForm.controls.quantity.value ?? 0);
    return total - (total * (this.entryForm.controls.discount.value ?? 0) / 100);
  }

  protected get totalPrice(): number {
    return (this.entryForm.controls.standardPrice.value ?? 0) *
      (this.entryForm.controls.quantity.value ?? 0)
  }

  protected get totalLineCost(): number {
    return this.lines().reduce((total, line) => total + line.totalCost, 0);
  }

  protected get grossTotal(): number {
    return this.lines().reduce(
      (total, line) => total + line.standardPrice * line.quantity,
      0,
    );
  }

  protected get totalDiscount(): number {
    return this.lines().reduce((total, line) => total + line.totalCost - line.totalPrice, 0);
  }

  protected get netTotal(): number {
    return this.grossTotal - this.totalLineCost;
  }

  protected get totalQuantity(): number {
    return this.lines().reduce((total, line) => total + line.quantity, 0);
  }

  protected onItemChange(): void {
    this.entryForm.controls.batch.setValue('');
    this.entryForm.controls.batch.markAsUntouched();
    this.statusMessage.set('');
  }

  protected addLine(): void {
    this.statusMessage.set('');
    if (this.entryForm.invalid) {
      this.entryForm.markAllAsTouched();
      this.statusMessage.set('Complete all required fields with valid values before adding the item.');
      return;
    }

    const { item, batch, standardCost, standardPrice, quantity, discount } =
      this.entryForm.getRawValue();
    const cost = standardCost ?? 0;
    const price = standardPrice ?? 0;
    const qty = quantity ?? 0;
    const discountAmount = discount ?? 0;
    const discountValue = cost * qty * (discountAmount / 100);

    if (discountAmount > price * qty) {
      this.statusMessage.set('Discount cannot exceed the line item selling total.');
      return;
    }

    const itemName = this.items.find((option) => option.id === item)?.name;
    if (!itemName) {
      this.statusMessage.set('Select a valid item before adding the line.');
      return;
    }

    this.lines.update((current) => [
      ...current,
      {
        id: this.nextLineId++,
        item: itemName,
        batch,
        standardCost: cost,
        standardPrice: price,
        margin: price - cost,
        quantity: qty,
        discount: discountAmount,
        totalCost: cost * qty - discountValue,
        totalPrice: price * qty,
      },
    ]);
    this.entryForm.reset({
      item: '',
      batch: '',
      standardCost: null,
      standardPrice: null,
      quantity: 1,
      discount: 0,
    });
  }

  protected removeLine(id: number): void {
    this.lines.update((current) => current.filter((line) => line.id !== id));
  }

  protected logout(): void {
    this.authService.logout();
    void this.router.navigate(['/']);
  }
}
