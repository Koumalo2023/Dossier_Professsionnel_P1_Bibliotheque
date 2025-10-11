import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { IconComponent } from '../../atoms/icons/icon.component';


@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [CommonModule, IconComponent, ReactiveFormsModule],
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.scss']
})
export class SearchBarComponent {
  @Input() placeholder: string = 'Rechercher un livre, un auteur...';
  @Input() disabled = false;
  @Input() showFilters = false;
  @Output() search = new EventEmitter<string>();
  @Output() filterChange = new EventEmitter<{ category?: string; availableOnly?: boolean }>();

  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  searchControl = new FormControl('');
  isFocused = false;
  showAdvancedFilters = false;
  selectedCategory: string | null = null;
  availableOnly = false;

  categories = [
    'Roman', 'Science-fiction', 'Biographie', 'Poésie', 'Essai', 'Jeunesse', 'Policier', 'Histoire'
  ];

  constructor() {
    // Émission débouncée de la recherche
    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(value => {
      this.search.emit(value?.trim() || '');
    });
  }

  onFocus(): void {
    this.isFocused = true;
  }

  onBlur(): void {
    // Léger délai pour permettre le clic sur les filtres
    setTimeout(() => {
      if (!this.showAdvancedFilters) {
        this.isFocused = false;
      }
    }, 150);
  }

  toggleFilters(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
    if (this.showAdvancedFilters) {
      this.isFocused = true;
    }
  }

  onCategoryChange(category: string | null): void {
    this.selectedCategory = category;
    this.emitFilters();
  }

  onAvailableOnlyChange(): void {
    this.availableOnly = !this.availableOnly;
    this.emitFilters();
  }

  private emitFilters(): void {
    this.filterChange.emit({
      category: this.selectedCategory || undefined,
      availableOnly: this.availableOnly
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.searchInput.nativeElement.focus();
  }
}