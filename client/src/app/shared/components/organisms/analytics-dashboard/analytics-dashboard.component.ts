import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnalyticsOverview, PopularCategory, TopBook } from '../../../../core/models/analytics.model';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';

interface StatItem {
  label: string;
  icon: string;
  color: 'primary' | 'accent' | 'success' | 'text-secondary';
  key: keyof AnalyticsOverview;
}

@Component({
  selector: 'app-analytics-dashboard',
  standalone: true,
  imports: [CommonModule, TypographyComponent, IconComponent, HeadingComponent],
  templateUrl: './analytics-dashboard.component.html',
  styleUrl: './analytics-dashboard.component.scss'
})
export class AnalyticsDashboardComponent {
  @Input() overview: AnalyticsOverview | null = null;
  @Input() popularCategories: PopularCategory[] = [];
  @Input() topBooks: TopBook[] = [];
  @Input() loading = false;

  // Définition des éléments de statistiques
  statItems: StatItem[] = [
    { label: 'Livres total', icon: 'fa-book', color: 'primary', key: 'totalBooks' },
    { label: 'Utilisateurs', icon: 'fa-users', color: 'accent', key: 'totalUsers' },
    { label: 'Emprunts actifs', icon: 'fa-book-open', color: 'success', key: 'activeLoans' },
    { label: 'Réservations', icon: 'fa-calendar-alt', color: 'text-secondary', key: 'pendingReservations' },
    { label: 'Emprunts aujourd\'hui', icon: 'fa-chart-line', color: 'primary', key: 'todayLoans' },
    { label: 'Croissance mensuelle', icon: 'fa-chart-pie', color: 'accent', key: 'monthlyGrowth' }
  ];

  // Mock data pour démo visuelle
  get mockOverview(): AnalyticsOverview {
    return {
      totalBooks: 1248,
      totalUsers: 842,
      activeLoans: 327,
      pendingReservations: 42,
      todayLoans: 18,
      monthlyGrowth: 12.5
    };
  }

  get mockCategories(): PopularCategory[] {
    return [
      { categoryName: 'Roman', borrowCount: 420, percentage: 34 },
      { categoryName: 'Science-fiction', borrowCount: 298, percentage: 24 },
      { categoryName: 'Biographie', borrowCount: 187, percentage: 15 },
      { categoryName: 'Jeunesse', borrowCount: 156, percentage: 13 }
    ];
  }

  get mockTopBooks(): TopBook[] {
    return [
      { bookId: '1', title: 'Le Petit Prince', author: 'Antoine de Saint-Exupéry', borrowCount: 87 },
      { bookId: '2', title: '1984', author: 'George Orwell', borrowCount: 76 },
      { bookId: '3', title: 'Dune', author: 'Frank Herbert', borrowCount: 68 }
    ];
  }

  getStatValue(key: string): string | number {
    const source = this.overview || this.mockOverview;
    switch (key) {
      case 'totalBooks': return source.totalBooks;
      case 'totalUsers': return source.totalUsers;
      case 'activeLoans': return source.activeLoans;
      case 'pendingReservations': return source.pendingReservations;
      case 'todayLoans': return source.todayLoans;
      case 'monthlyGrowth': return source.monthlyGrowth;
      default: return '--';
    }
  }
}
