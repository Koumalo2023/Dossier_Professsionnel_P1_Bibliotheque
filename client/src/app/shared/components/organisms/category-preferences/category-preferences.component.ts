import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { UserCategoryPreference } from '../../../../core/models/user-profile.model';
import { Category } from '../../../../core/models/book.model';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';

@Component({
  selector: 'app-category-preferences',
   standalone: true,
  imports: [TypographyComponent, HeadingComponent],
  templateUrl: './category-preferences.component.html',
  styleUrl: './category-preferences.component.scss'
})
export class CategoryPreferencesComponent {
 @Input() availableCategories: Category[] = [];
  @Input() userPreferences: UserCategoryPreference[] = [];
  @Input() loading = false;

  @Output() preferencesChange = new EventEmitter<UserCategoryPreference[]>();

  selectedPreferences: { [categoryId: string]: boolean } = {};

  ngOnInit(): void {
    // Initialise les catégories sélectionnées à partir des préférences utilisateur
    this.selectedPreferences = {};
    this.availableCategories.forEach(cat => {
      this.selectedPreferences[cat.id] = this.userPreferences.some(p => p.categoryId === cat.id);
    });
  }

  toggleCategory(categoryId: string): void {
    this.selectedPreferences[categoryId] = !this.selectedPreferences[categoryId];
    this.emitPreferences();
  }

  private emitPreferences(): void {
    const preferences = this.availableCategories
      .filter(cat => this.selectedPreferences[cat.id])
      .map(cat => ({
        id: `temp-${cat.id}`, // ID temporaire pour la création
        categoryId: cat.id,
        categoryName: cat.name,
        preferenceScore: 1, // valeur par défaut
        interactionCount: 0,
        lastInteracted: new Date().toISOString()
      }));

    this.preferencesChange.emit(preferences);
  }

  isCategorySelected(categoryId: string): boolean {
    return this.selectedPreferences[categoryId] || false;
  }
}
