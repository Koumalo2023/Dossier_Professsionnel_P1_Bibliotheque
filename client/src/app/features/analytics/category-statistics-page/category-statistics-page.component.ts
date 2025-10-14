import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnalyticsService } from '../../../core/services/api/analytics.service';
import { ChartComponent } from '../../../shared/components/molecules/chart/chart.component';
import { AnalyticsPeriod } from '../../../core/models/analytics.model';

interface CategoryStat {
  category: string;
  borrowCount: number;
  percentage: number;
  averageLoanDuration: number;
  reservationCount: number;
  trend: 'up' | 'down' | 'stable';
}

@Component({
  selector: 'app-category-statistics-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ChartComponent
  ],
  templateUrl: './category-statistics-page.component.html'
})
export class CategoryStatisticsPageComponent implements OnInit {
  private analyticsService = inject(AnalyticsService);

  // Données statistiques
  categoryStats: CategoryStat[] = [];
  loading = false;
  error: string | null = null;

  // Filtres
  periodFilter: AnalyticsPeriod = AnalyticsPeriod.MONTH;
  sortBy: string = 'borrowCount';
  sortDirection: 'asc' | 'desc' = 'desc';
  searchTerm: string = '';

  // Options de période
  readonly periodOptions = [
    { value: AnalyticsPeriod.WEEK, label: 'Cette semaine' },
    { value: AnalyticsPeriod.MONTH, label: 'Ce mois' },
    { value: AnalyticsPeriod.YEAR, label: 'Cette année' }
  ];

  // Options de tri
  readonly sortOptions = [
    { value: 'borrowCount', label: 'Nombre d\'emprunts' },
    { value: 'percentage', label: 'Pourcentage' },
    { value: 'averageLoanDuration', label: 'Durée moyenne' },
    { value: 'reservationCount', label: 'Réservations' },
    { value: 'category', label: 'Catégorie' }
  ];

  ngOnInit() {
    this.loadCategoryStats();
  }

  loadCategoryStats(): void {
    this.loading = true;
    this.error = null;

    this.analyticsService.getPopularCategories(this.periodFilter).subscribe({
      next: (data) => {
        // Transformer les données pour correspondre à notre interface
        this.categoryStats = data.map(category => ({
          category: category.categoryName,
          borrowCount: category.borrowCount,
          percentage: category.percentage,
          averageLoanDuration: Math.floor(Math.random() * 30) + 7, // Données simulées
          reservationCount: Math.floor(category.borrowCount * 0.1), // Données simulées
          trend: this.getRandomTrend()
        }));
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Erreur lors du chargement des statistiques par catégorie';
        this.loading = false;
        console.error('Error loading category stats:', error);
      }
    });
  }

  getRandomTrend(): 'up' | 'down' | 'stable' {
    const trends: ('up' | 'down' | 'stable')[] = ['up', 'down', 'stable'];
    return trends[Math.floor(Math.random() * trends.length)];
  }

  onPeriodFilterChange(): void {
    this.loadCategoryStats();
  }

  onSortChange(): void {
    this.sortData();
  }

  onSearchChange(): void {
    // La recherche se fait côté client, pas besoin de recharger les données
  }

  sortData(): void {
    this.categoryStats.sort((a, b) => {
      let valueA: any = a[this.sortBy as keyof CategoryStat];
      let valueB: any = b[this.sortBy as keyof CategoryStat];

      if (typeof valueA === 'string') {
        valueA = valueA.toLowerCase();
        valueB = valueB.toLowerCase();
      }

      if (valueA < valueB) {
        return this.sortDirection === 'asc' ? -1 : 1;
      }
      if (valueA > valueB) {
        return this.sortDirection === 'asc' ? 1 : -1;
      }
      return 0;
    });
  }

  toggleSortDirection(): void {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    this.sortData();
  }

  getFilteredStats(): CategoryStat[] {
    let filtered = this.categoryStats;

    // Appliquer la recherche
    if (this.searchTerm) {
      filtered = filtered.filter(stat => 
        stat.category.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }

    return filtered;
  }

  getTotalBorrows(): number {
    return this.categoryStats.reduce((sum, stat) => sum + stat.borrowCount, 0);
  }

  getTopCategory(): string {
    if (this.categoryStats.length === 0) return 'N/A';
    const topCategory = [...this.categoryStats].sort((a, b) => b.borrowCount - a.borrowCount)[0];
    return topCategory.category;
  }

  getCategoryDistributionChartData(): any {
    if (this.categoryStats.length === 0) return null;

    return {
      labels: this.categoryStats.map(stat => stat.category),
      datasets: [
        {
          data: this.categoryStats.map(stat => stat.percentage),
          backgroundColor: [
            '#4f46e5', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6',
            '#06b6d4', '#84cc16', '#f97316', '#6366f1', '#ec4899',
            '#14b8a6', '#f43f5e', '#8b5cf6', '#06b6d4', '#84cc16'
          ],
          borderWidth: 2,
          borderColor: '#ffffff'
        }
      ]
    };
  }

  getBorrowTrendsChartData(): any {
    if (this.categoryStats.length === 0) return null;

    return {
      labels: this.categoryStats.map(stat => stat.category),
      datasets: [
        {
          label: 'Nombre d\'emprunts',
          data: this.categoryStats.map(stat => stat.borrowCount),
          backgroundColor: 'rgba(79, 70, 229, 0.8)',
          borderColor: '#4f46e5',
          borderWidth: 2
        }
      ]
    };
  }

  getTrendIcon(trend: 'up' | 'down' | 'stable'): string {
    switch (trend) {
      case 'up': return '📈';
      case 'down': return '📉';
      case 'stable': return '➡️';
      default: return '➡️';
    }
  }

  getTrendColor(trend: 'up' | 'down' | 'stable'): string {
    switch (trend) {
      case 'up': return 'text-green-600';
      case 'down': return 'text-red-600';
      case 'stable': return 'text-gray-600';
      default: return 'text-gray-600';
    }
  }

  exportCategoryReport(): void {
    // Simuler l'export (à implémenter côté backend)
    console.log('Exporting category report with period:', this.periodFilter);
    
    // Pour l'instant, on simule un délai et on affiche un message
    setTimeout(() => {
      alert('Fonctionnalité d\'export en cours de développement. Les données sont disponibles via les API.');
    }, 1000);
  }

  // Méthodes pour le résumé statistique
  getTopCategoryBorrowCount(): number {
    const topCategory = this.getTopCategory();
    const stat = this.getFilteredStats().find(s => s.category === topCategory);
    return stat ? stat.borrowCount : 0;
  }

  getAverageLoanDuration(): number {
    const stats = this.getFilteredStats();
    if (stats.length === 0) return 0;
    const totalDuration = stats.reduce((sum, s) => sum + s.averageLoanDuration, 0);
    return totalDuration / stats.length;
  }

  getReservationRate(): number {
    const totalReservations = this.getFilteredStats().reduce((sum, s) => sum + s.reservationCount, 0);
    const totalBorrows = this.getTotalBorrows();
    if (totalBorrows === 0) return 0;
    return (totalReservations / totalBorrows) * 100;
  }
}