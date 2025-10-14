import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnalyticsService } from '../../../core/services/api/analytics.service';
import { AnalyticsDashboardComponent } from '../../../shared/components/organisms/analytics-dashboard/analytics-dashboard.component';
import { ChartComponent } from '../../../shared/components/molecules/chart/chart.component';

@Component({
  selector: 'app-analytics-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AnalyticsDashboardComponent,
    ChartComponent
  ],
  templateUrl: './analytics-dashboard-page.component.html'
})
export class AnalyticsDashboardPageComponent implements OnInit {
  private analyticsService = inject(AnalyticsService);

  // Données statistiques
  overviewData: any = null;
  statisticsData: any = null;
  trendsData: any = null;
  loading = false;
  error: string | null = null;

  // Filtres de période
  periodFilter: string = 'month';
  startDate: string = '';
  endDate: string = '';

  // Options de période
  readonly periodOptions = [
    { value: 'today', label: 'Aujourd\'hui' },
    { value: 'week', label: 'Cette semaine' },
    { value: 'month', label: 'Ce mois' },
    { value: 'quarter', label: 'Ce trimestre' },
    { value: 'year', label: 'Cette année' },
    { value: 'custom', label: 'Période personnalisée' }
  ];

  ngOnInit() {
    this.initializeDates();
    this.loadDashboardData();
  }

  initializeDates(): void {
    const today = new Date();
    const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
    
    this.endDate = today.toISOString().split('T')[0];
    this.startDate = firstDayOfMonth.toISOString().split('T')[0];
  }

  loadDashboardData(): void {
    this.loading = true;
    this.error = null;

    // Charger les données en parallèle
    Promise.all([
      this.loadOverview(),
      this.loadStatistics(),
      this.loadTrends()
    ]).then(() => {
      this.loading = false;
    }).catch((error) => {
      this.error = 'Erreur lors du chargement des données analytiques';
      this.loading = false;
      console.error('Error loading analytics data:', error);
    });
  }

  loadOverview(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.analyticsService.getOverview().subscribe({
        next: (data) => {
          this.overviewData = data;
          resolve();
        },
        error: (error) => {
          console.error('Error loading overview:', error);
          reject(error);
        }
      });
    });
  }

  loadStatistics(): Promise<void> {
    const filters = this.buildFilters();
    return new Promise((resolve, reject) => {
      // Charger les catégories populaires
      this.analyticsService.getPopularCategories(filters.period).subscribe({
        next: (data) => {
          this.statisticsData = { ...this.statisticsData, popularCategories: data };
        },
        error: (error) => {
          console.error('Error loading popular categories:', error);
        }
      });

      // Charger les statistiques de réservation
      this.analyticsService.getReservationStats().subscribe({
        next: (data) => {
          this.statisticsData = { ...this.statisticsData, reservationStats: data };
        },
        error: (error) => {
          console.error('Error loading reservation stats:', error);
        }
      });

      // Charger les livres les plus empruntés
      this.analyticsService.getTopBooks(10).subscribe({
        next: (data) => {
          this.statisticsData = { ...this.statisticsData, topBooks: data };
          resolve();
        },
        error: (error) => {
          console.error('Error loading top books:', error);
          reject(error);
        }
      });
    });
  }

  loadTrends(): Promise<void> {
    const filters = this.buildFilters();
    return new Promise((resolve, reject) => {
      // Charger les tendances d'emprunt
      this.analyticsService.getBorrowTrends(filters.period).subscribe({
        next: (data) => {
          this.trendsData = { ...this.trendsData, borrowTrends: data };
          resolve();
        },
        error: (error) => {
          console.error('Error loading borrow trends:', error);
          reject(error);
        }
      });
    });
  }

  buildFilters(): any {
    const filters: any = {
      period: this.periodFilter
    };

    if (this.periodFilter === 'custom' && this.startDate && this.endDate) {
      filters.startDate = this.startDate;
      filters.endDate = this.endDate;
    }

    return filters;
  }

  onPeriodFilterChange(): void {
    this.updateDatesForPeriod();
    this.loadDashboardData();
  }

  onCustomDateChange(): void {
    if (this.periodFilter === 'custom') {
      this.loadDashboardData();
    }
  }

  updateDatesForPeriod(): void {
    const today = new Date();
    let startDate = new Date();

    switch (this.periodFilter) {
      case 'today':
        startDate = new Date(today);
        break;
      case 'week':
        startDate = new Date(today);
        startDate.setDate(today.getDate() - 7);
        break;
      case 'month':
        startDate = new Date(today.getFullYear(), today.getMonth(), 1);
        break;
      case 'quarter':
        const quarter = Math.floor(today.getMonth() / 3);
        startDate = new Date(today.getFullYear(), quarter * 3, 1);
        break;
      case 'year':
        startDate = new Date(today.getFullYear(), 0, 1);
        break;
      case 'custom':
        // Les dates sont déjà gérées par l'utilisateur
        return;
    }

    this.startDate = startDate.toISOString().split('T')[0];
    this.endDate = today.toISOString().split('T')[0];
  }

  exportReport(): void {
    const filters = this.buildFilters();
    
    // Simuler l'export (à implémenter côté backend)
    console.log('Exporting report with filters:', filters);
    
    // Pour l'instant, on simule un délai et on affiche un message
    setTimeout(() => {
      // En attendant l'implémentation backend, on affiche un message
      alert('Fonctionnalité d\'export en cours de développement. Les données sont disponibles via les API.');
    }, 1000);
  }

  // Méthodes utilitaires pour formater les données des graphiques
  getLoanTrendsChartData(): any {
    if (!this.trendsData?.loanTrends) return null;

    return {
      labels: this.trendsData.loanTrends.map((trend: any) => trend.period),
      datasets: [
        {
          label: 'Emprunts',
          data: this.trendsData.loanTrends.map((trend: any) => trend.count),
          borderColor: '#4f46e5',
          backgroundColor: 'rgba(79, 70, 229, 0.1)',
          tension: 0.4
        }
      ]
    };
  }

  getCategoryDistributionChartData(): any {
    if (!this.statisticsData?.categoryDistribution) return null;

    return {
      labels: this.statisticsData.categoryDistribution.map((cat: any) => cat.category),
      datasets: [
        {
          data: this.statisticsData.categoryDistribution.map((cat: any) => cat.count),
          backgroundColor: [
            '#4f46e5', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6',
            '#06b6d4', '#84cc16', '#f97316', '#6366f1', '#ec4899'
          ]
        }
      ]
    };
  }

  getUserActivityChartData(): any {
    if (!this.trendsData?.userActivity) return null;

    return {
      labels: this.trendsData.userActivity.map((activity: any) => activity.period),
      datasets: [
        {
          label: 'Utilisateurs actifs',
          data: this.trendsData.userActivity.map((activity: any) => activity.activeUsers),
          borderColor: '#10b981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          tension: 0.4
        }
      ]
    };
  }

  getReservationConversionChartData(): any {
    if (!this.statisticsData?.reservationConversion) return null;

    return {
      labels: ['Réservations créées', 'Réservations converties'],
      datasets: [
        {
          data: [
            this.statisticsData.reservationConversion.totalReservations,
            this.statisticsData.reservationConversion.convertedReservations
          ],
          backgroundColor: ['#f59e0b', '#10b981']
        }
      ]
    };
  }

  // Méthodes pour les indicateurs clés
  getTotalLoans(): number {
    return this.overviewData?.totalLoans || 0;
  }

  getActiveLoans(): number {
    return this.overviewData?.activeLoans || 0;
  }

  getOverdueLoans(): number {
    return this.overviewData?.overdueLoans || 0;
  }

  getTotalUsers(): number {
    return this.overviewData?.totalUsers || 0;
  }

  getActiveUsers(): number {
    return this.overviewData?.activeUsers || 0;
  }

  getPopularCategory(): string {
    return this.overviewData?.popularCategory || 'N/A';
  }

  getLoanIncrease(): number {
    return this.trendsData?.loanIncrease || 0;
  }

  getUserGrowth(): number {
    return this.trendsData?.userGrowth || 0;
  }
}