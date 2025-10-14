import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnalyticsService } from '../../../core/services/api/analytics.service';
import { AnalyticsPeriod } from '../../../core/models/analytics.model';
import { ChartComponent } from '../../../shared/components/molecules/chart/chart.component';

interface LoanTrend {
  period: string;
  borrowCount: number;
  returnCount: number;
  overdueCount: number;
  averageDuration: number;
  trend: number;
}

interface TrendComparison {
  currentPeriod: number;
  previousPeriod: number;
  change: number;
  changePercentage: number;
}

@Component({
  selector: 'app-loan-trends-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ChartComponent
  ],
  templateUrl: './loan-trends-page.component.html'
})
export class LoanTrendsPageComponent implements OnInit {
  private analyticsService = inject(AnalyticsService);

  // Données des tendances
  loanTrends: LoanTrend[] = [];
  loading = false;
  error: string | null = null;

  // Filtres
  periodFilter: AnalyticsPeriod = AnalyticsPeriod.MONTH;
  viewType: 'daily' | 'weekly' | 'monthly' = 'daily';
  showProjection: boolean = false;

  // Options de période
  readonly periodOptions = [
    { value: AnalyticsPeriod.WEEK, label: 'Cette semaine' },
    { value: AnalyticsPeriod.MONTH, label: 'Ce mois' },
    { value: AnalyticsPeriod.YEAR, label: 'Cette année' }
  ];

  // Options de vue
  readonly viewOptions = [
    { value: 'daily', label: 'Vue quotidienne' },
    { value: 'weekly', label: 'Vue hebdomadaire' },
    { value: 'monthly', label: 'Vue mensuelle' }
  ];

  // Données de comparaison
  comparisonData: TrendComparison = {
    currentPeriod: 0,
    previousPeriod: 0,
    change: 0,
    changePercentage: 0
  };

  ngOnInit() {
    this.loadLoanTrends();
  }

  loadLoanTrends(): void {
    this.loading = true;
    this.error = null;

    this.analyticsService.getBorrowTrends(this.periodFilter).subscribe({
      next: (data) => {
        // Transformer les données pour correspondre à notre interface
        this.loanTrends = data.map(trend => ({
          period: trend.period,
          borrowCount: trend.borrowCount,
          returnCount: Math.floor(trend.borrowCount * 0.8), // Données simulées
          overdueCount: Math.floor(trend.borrowCount * 0.05), // Données simulées
          averageDuration: Math.floor(Math.random() * 20) + 7, // Données simulées
          trend: trend.trend
        }));
        
        this.calculateComparisonData();
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Erreur lors du chargement des tendances d\'emprunt';
        this.loading = false;
        console.error('Error loading loan trends:', error);
      }
    });
  }

  calculateComparisonData(): void {
    if (this.loanTrends.length < 2) return;

    const currentPeriodTotal = this.loanTrends.reduce((sum, trend) => sum + trend.borrowCount, 0);
    const previousPeriodTotal = Math.floor(currentPeriodTotal * 0.85); // Données simulées

    this.comparisonData = {
      currentPeriod: currentPeriodTotal,
      previousPeriod: previousPeriodTotal,
      change: currentPeriodTotal - previousPeriodTotal,
      changePercentage: ((currentPeriodTotal - previousPeriodTotal) / previousPeriodTotal) * 100
    };
  }

  onPeriodFilterChange(): void {
    this.loadLoanTrends();
  }

  onViewTypeChange(): void {
    // Recharger ou recalculer les données selon la vue
    this.loadLoanTrends();
  }

  getTotalBorrows(): number {
    return this.loanTrends.reduce((sum, trend) => sum + trend.borrowCount, 0);
  }

  getTotalReturns(): number {
    return this.loanTrends.reduce((sum, trend) => sum + trend.returnCount, 0);
  }

  getTotalOverdue(): number {
    return this.loanTrends.reduce((sum, trend) => sum + trend.overdueCount, 0);
  }

  getAverageDuration(): number {
    if (this.loanTrends.length === 0) return 0;
    const total = this.loanTrends.reduce((sum, trend) => sum + trend.averageDuration, 0);
    return total / this.loanTrends.length;
  }

  getCompletionRate(): number {
    const totalBorrows = this.getTotalBorrows();
    if (totalBorrows === 0) return 0;
    return (this.getTotalReturns() / totalBorrows) * 100;
  }

  getOverdueRate(): number {
    const totalBorrows = this.getTotalBorrows();
    if (totalBorrows === 0) return 0;
    return (this.getTotalOverdue() / totalBorrows) * 100;
  }

  getTrendIcon(change: number): string {
    if (change > 0) return '📈';
    if (change < 0) return '📉';
    return '➡️';
  }

  getTrendColor(change: number): string {
    if (change > 0) return 'text-green-600';
    if (change < 0) return 'text-red-600';
    return 'text-gray-600';
  }

  getTrendLabel(change: number): string {
    if (change > 0) return 'En hausse';
    if (change < 0) return 'En baisse';
    return 'Stable';
  }

  getBorrowTrendsChartData(): any {
    if (this.loanTrends.length === 0) return null;

    return {
      labels: this.loanTrends.map(trend => trend.period),
      datasets: [
        {
          label: 'Emprunts',
          data: this.loanTrends.map(trend => trend.borrowCount),
          borderColor: '#4f46e5',
          backgroundColor: 'rgba(79, 70, 229, 0.1)',
          tension: 0.4,
          fill: true
        },
        {
          label: 'Retours',
          data: this.loanTrends.map(trend => trend.returnCount),
          borderColor: '#10b981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    };
  }

  getOverdueTrendsChartData(): any {
    if (this.loanTrends.length === 0) return null;

    return {
      labels: this.loanTrends.map(trend => trend.period),
      datasets: [
        {
          label: 'Retards',
          data: this.loanTrends.map(trend => trend.overdueCount),
          borderColor: '#ef4444',
          backgroundColor: 'rgba(239, 68, 68, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    };
  }

  getDurationTrendsChartData(): any {
    if (this.loanTrends.length === 0) return null;

    return {
      labels: this.loanTrends.map(trend => trend.period),
      datasets: [
        {
          label: 'Durée moyenne (jours)',
          data: this.loanTrends.map(trend => trend.averageDuration),
          borderColor: '#f59e0b',
          backgroundColor: 'rgba(245, 158, 11, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    };
  }

  getComparisonChartData(): any {
    if (this.loanTrends.length === 0) return null;

    // Simuler les données de la période précédente
    const previousPeriodData = this.loanTrends.map(trend => 
      Math.floor(trend.borrowCount * (0.7 + Math.random() * 0.3))
    );

    return {
      labels: this.loanTrends.map(trend => trend.period),
      datasets: [
        {
          label: 'Période actuelle',
          data: this.loanTrends.map(trend => trend.borrowCount),
          backgroundColor: 'rgba(79, 70, 229, 0.8)',
          borderColor: '#4f46e5',
          borderWidth: 2
        },
        {
          label: 'Période précédente',
          data: previousPeriodData,
          backgroundColor: 'rgba(156, 163, 175, 0.8)',
          borderColor: '#9ca3af',
          borderWidth: 2
        }
      ]
    };
  }

  getPeakPeriod(): string {
    if (this.loanTrends.length === 0) return 'N/A';
    const peak = this.loanTrends.reduce((max, trend) => 
      trend.borrowCount > max.borrowCount ? trend : max
    );
    return peak.period;
  }

  getLowestPeriod(): string {
    if (this.loanTrends.length === 0) return 'N/A';
    const lowest = this.loanTrends.reduce((min, trend) => 
      trend.borrowCount < min.borrowCount ? trend : min
    );
    return lowest.period;
  }

  exportTrendsReport(): void {
    // Simuler l'export (à implémenter côté backend)
    console.log('Exporting trends report with period:', this.periodFilter);
    
    // Pour l'instant, on simule un délai et on affiche un message
    setTimeout(() => {
      alert('Fonctionnalité d\'export en cours de développement. Les données sont disponibles via les API.');
    }, 1000);
  }

  // Méthodes pour le template
  getPeakPeriodBorrowCount(): number {
    const peakPeriod = this.getPeakPeriod();
    const trend = this.loanTrends.find(t => t.period === peakPeriod);
    return trend ? trend.borrowCount : 0;
  }

  getLowestPeriodBorrowCount(): number {
    const lowestPeriod = this.getLowestPeriod();
    const trend = this.loanTrends.find(t => t.period === lowestPeriod);
    return trend ? trend.borrowCount : 0;
  }

  getAbsoluteChangePercentage(): number {
    return Math.abs(this.comparisonData.changePercentage);
  }
}