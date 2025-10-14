import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnalyticsService } from '../../../core/services/api/analytics.service';
import { AnalyticsPeriod } from '../../../core/models/analytics.model';
import { ChartComponent } from '../../../shared/components/molecules/chart/chart.component';

interface UserActivity {
  period: string;
  activeUsers: number;
  newUsers: number;
  returningUsers: number;
  averageSessions: number;
  engagementRate: number;
}

interface UserSegment {
  segment: string;
  count: number;
  percentage: number;
  trend: 'up' | 'down' | 'stable';
}

interface ActivityMetric {
  label: string;
  value: number;
  change: number;
  trend: 'up' | 'down' | 'stable';
}

@Component({
  selector: 'app-user-activity-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ChartComponent
  ],
  templateUrl: './user-activity-page.component.html'
})
export class UserActivityPageComponent implements OnInit {
  private analyticsService = inject(AnalyticsService);

  // Données d'activité
  userActivity: UserActivity[] = [];
  activeUsers: any[] = [];
  userSegments: UserSegment[] = [];
  loading = false;
  error: string | null = null;

  // Filtres
  periodFilter: AnalyticsPeriod = AnalyticsPeriod.MONTH;
  userTypeFilter: 'all' | 'new' | 'returning' = 'all';
  activityLevelFilter: 'all' | 'high' | 'medium' | 'low' = 'all';

  // Options de période
  readonly periodOptions = [
    { value: AnalyticsPeriod.WEEK, label: 'Cette semaine' },
    { value: AnalyticsPeriod.MONTH, label: 'Ce mois' },
    { value: AnalyticsPeriod.YEAR, label: 'Cette année' }
  ];

  // Options de type d'utilisateur
  readonly userTypeOptions = [
    { value: 'all', label: 'Tous les utilisateurs' },
    { value: 'new', label: 'Nouveaux utilisateurs' },
    { value: 'returning', label: 'Utilisateurs de retour' }
  ];

  // Options de niveau d'activité
  readonly activityLevelOptions = [
    { value: 'all', label: 'Tous les niveaux' },
    { value: 'high', label: 'Activité élevée' },
    { value: 'medium', label: 'Activité moyenne' },
    { value: 'low', label: 'Activité faible' }
  ];

  // Métriques principales
  activityMetrics: ActivityMetric[] = [];

  ngOnInit() {
    this.loadUserActivity();
  }

  loadUserActivity(): void {
    this.loading = true;
    this.error = null;

    // Charger les utilisateurs actifs
    this.analyticsService.getActiveUsers(20).subscribe({
      next: (data) => {
        this.activeUsers = data;
        this.generateActivityData();
        this.generateSegments();
        this.calculateMetrics();
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Erreur lors du chargement de l\'activité des utilisateurs';
        this.loading = false;
        console.error('Error loading user activity:', error);
      }
    });
  }

  generateActivityData(): void {
    // Générer des données d'activité simulées basées sur les utilisateurs actifs
    const periods = this.getPeriodsForFilter();
    this.userActivity = periods.map(period => {
      const baseActiveUsers = Math.floor(Math.random() * 50) + 20;
      return {
        period,
        activeUsers: baseActiveUsers,
        newUsers: Math.floor(baseActiveUsers * 0.2),
        returningUsers: Math.floor(baseActiveUsers * 0.8),
        averageSessions: Math.floor(Math.random() * 5) + 1,
        engagementRate: Math.floor(Math.random() * 40) + 60
      };
    });
  }

  generateSegments(): void {
    this.userSegments = [
      {
        segment: 'Utilisateurs très actifs',
        count: Math.floor(this.activeUsers.length * 0.3),
        percentage: 30,
        trend: 'up'
      },
      {
        segment: 'Utilisateurs modérés',
        count: Math.floor(this.activeUsers.length * 0.5),
        percentage: 50,
        trend: 'stable'
      },
      {
        segment: 'Utilisateurs occasionnels',
        count: Math.floor(this.activeUsers.length * 0.2),
        percentage: 20,
        trend: 'down'
      }
    ];
  }

  calculateMetrics(): void {
    const totalActiveUsers = this.userActivity.reduce((sum, activity) => sum + activity.activeUsers, 0);
    const totalNewUsers = this.userActivity.reduce((sum, activity) => sum + activity.newUsers, 0);
    const totalReturningUsers = this.userActivity.reduce((sum, activity) => sum + activity.returningUsers, 0);
    const averageEngagement = this.userActivity.reduce((sum, activity) => sum + activity.engagementRate, 0) / this.userActivity.length;

    this.activityMetrics = [
      {
        label: 'Utilisateurs Actifs',
        value: totalActiveUsers,
        change: 12,
        trend: 'up'
      },
      {
        label: 'Nouveaux Utilisateurs',
        value: totalNewUsers,
        change: 8,
        trend: 'up'
      },
      {
        label: 'Utilisateurs de Retour',
        value: totalReturningUsers,
        change: -3,
        trend: 'down'
      },
      {
        label: 'Taux d\'Engagement',
        value: averageEngagement,
        change: 5,
        trend: 'up'
      }
    ];
  }

  getPeriodsForFilter(): string[] {
    switch (this.periodFilter) {
      case AnalyticsPeriod.WEEK:
        return ['Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam', 'Dim'];
      case AnalyticsPeriod.MONTH:
        return ['Sem 1', 'Sem 2', 'Sem 3', 'Sem 4'];
      case AnalyticsPeriod.YEAR:
        return ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun', 'Jul', 'Aoû', 'Sep', 'Oct', 'Nov', 'Déc'];
      default:
        return ['Période 1', 'Période 2', 'Période 3'];
    }
  }

  onPeriodFilterChange(): void {
    this.loadUserActivity();
  }

  onUserTypeFilterChange(): void {
    this.loadUserActivity();
  }

  onActivityLevelFilterChange(): void {
    this.loadUserActivity();
  }

  getTotalActiveUsers(): number {
    return this.userActivity.reduce((sum, activity) => sum + activity.activeUsers, 0);
  }

  getTotalNewUsers(): number {
    return this.userActivity.reduce((sum, activity) => sum + activity.newUsers, 0);
  }

  getTotalReturningUsers(): number {
    return this.userActivity.reduce((sum, activity) => sum + activity.returningUsers, 0);
  }

  getAverageEngagement(): number {
    if (this.userActivity.length === 0) return 0;
    const total = this.userActivity.reduce((sum, activity) => sum + activity.engagementRate, 0);
    return total / this.userActivity.length;
  }

  getAverageSessions(): number {
    if (this.userActivity.length === 0) return 0;
    const total = this.userActivity.reduce((sum, activity) => sum + activity.averageSessions, 0);
    return total / this.userActivity.length;
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

  getTrendLabel(trend: 'up' | 'down' | 'stable'): string {
    switch (trend) {
      case 'up': return 'En hausse';
      case 'down': return 'En baisse';
      case 'stable': return 'Stable';
      default: return 'Stable';
    }
  }

  getUserActivityChartData(): any {
    if (this.userActivity.length === 0) return null;

    return {
      labels: this.userActivity.map(activity => activity.period),
      datasets: [
        {
          label: 'Utilisateurs Actifs',
          data: this.userActivity.map(activity => activity.activeUsers),
          borderColor: '#4f46e5',
          backgroundColor: 'rgba(79, 70, 229, 0.1)',
          tension: 0.4,
          fill: true
        },
        {
          label: 'Nouveaux Utilisateurs',
          data: this.userActivity.map(activity => activity.newUsers),
          borderColor: '#10b981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    };
  }

  getUserEngagementChartData(): any {
    if (this.userActivity.length === 0) return null;

    return {
      labels: this.userActivity.map(activity => activity.period),
      datasets: [
        {
          label: 'Taux d\'Engagement (%)',
          data: this.userActivity.map(activity => activity.engagementRate),
          borderColor: '#f59e0b',
          backgroundColor: 'rgba(245, 158, 11, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    };
  }

  getUserSegmentsChartData(): any {
    if (this.userSegments.length === 0) return null;

    return {
      labels: this.userSegments.map(segment => segment.segment),
      datasets: [
        {
          data: this.userSegments.map(segment => segment.count),
          backgroundColor: [
            '#4f46e5',
            '#10b981',
            '#f59e0b'
          ],
          borderWidth: 2,
          borderColor: '#ffffff'
        }
      ]
    };
  }

  getSessionActivityChartData(): any {
    if (this.userActivity.length === 0) return null;

    return {
      labels: this.userActivity.map(activity => activity.period),
      datasets: [
        {
          label: 'Sessions Moyennes',
          data: this.userActivity.map(activity => activity.averageSessions),
          borderColor: '#8b5cf6',
          backgroundColor: 'rgba(139, 92, 246, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    };
  }

  getFilteredActiveUsers(): any[] {
    let filtered = this.activeUsers;

    // Filtrer par type d'utilisateur
    if (this.userTypeFilter === 'new') {
      // Simuler le filtrage des nouveaux utilisateurs
      filtered = filtered.slice(0, Math.floor(filtered.length * 0.3));
    } else if (this.userTypeFilter === 'returning') {
      // Simuler le filtrage des utilisateurs de retour
      filtered = filtered.slice(Math.floor(filtered.length * 0.3));
    }

    // Filtrer par niveau d'activité
    if (this.activityLevelFilter !== 'all') {
      // Simuler le filtrage par niveau d'activité
      filtered = filtered.slice(0, Math.floor(filtered.length * 0.7));
    }

    return filtered;
  }

  getRetentionRate(): number {
    const totalUsers = this.getTotalActiveUsers();
    const returningUsers = this.getTotalReturningUsers();
    if (totalUsers === 0) return 0;
    return (returningUsers / totalUsers) * 100;
  }

  getGrowthRate(): number {
    if (this.userActivity.length < 2) return 0;
    const firstPeriod = this.userActivity[0].activeUsers;
    const lastPeriod = this.userActivity[this.userActivity.length - 1].activeUsers;
    if (firstPeriod === 0) return 0;
    return ((lastPeriod - firstPeriod) / firstPeriod) * 100;
  }

  exportUserActivityReport(): void {
    // Simuler l'export (à implémenter côté backend)
    console.log('Exporting user activity report with period:', this.periodFilter);
    
    // Pour l'instant, on simule un délai et on affiche un message
    setTimeout(() => {
      alert('Fonctionnalité d\'export en cours de développement. Les données sont disponibles via les API.');
    }, 1000);
  }

  // Méthodes utilitaires pour le template
  getActivityLevel(loanCount: number): string {
    if (loanCount >= 10) return 'Élevée';
    if (loanCount >= 5) return 'Moyenne';
    return 'Faible';
  }

  getActivityLevelClass(loanCount: number): string {
    if (loanCount >= 10) return 'level-high';
    if (loanCount >= 5) return 'level-medium';
    return 'level-low';
  }

  formatLastActivity(lastActivity: string): string {
    if (!lastActivity) return 'N/A';
    
    // Simuler le formatage de date
    const date = new Date(lastActivity);
    const now = new Date();
    const diffDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24));
    
    if (diffDays === 0) return 'Aujourd\'hui';
    if (diffDays === 1) return 'Hier';
    if (diffDays < 7) return `Il y a ${diffDays} jours`;
    if (diffDays < 30) return `Il y a ${Math.floor(diffDays / 7)} semaines`;
    return `Il y a ${Math.floor(diffDays / 30)} mois`;
  }

  getUserStatus(lastActivity: string): string {
    if (!lastActivity) return 'Inactif';
    
    const date = new Date(lastActivity);
    const now = new Date();
    const diffDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24));
    
    if (diffDays <= 1) return 'Très actif';
    if (diffDays <= 7) return 'Actif';
    if (diffDays <= 30) return 'Occasionnel';
    return 'Inactif';
  }

  getUserStatusClass(lastActivity: string): string {
    if (!lastActivity) return 'status-inactive';
    
    const date = new Date(lastActivity);
    const now = new Date();
    const diffDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24));
    
    if (diffDays <= 1) return 'status-very-active';
    if (diffDays <= 7) return 'status-active';
    if (diffDays <= 30) return 'status-occasional';
    return 'status-inactive';
  }
}