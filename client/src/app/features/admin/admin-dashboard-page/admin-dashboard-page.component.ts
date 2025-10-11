import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AnalyticsDashboardComponent } from '../../../shared/components/organisms/analytics-dashboard/analytics-dashboard.component';
import { ChartComponent } from '../../../../styles/shared/components/organims/chart/chart.component'; 
import { TypographyComponent } from '../../../shared/components/atoms/typography/typography.component';
import { ButtonComponent } from '../../../shared/components/atoms/button/button.component';
import { UserAvatarComponent } from '../../../shared/components/molecules/user-avatar/user-avatar.component';
import { HeadingComponent } from '../../../shared/components/atoms/heading/heading.component';

interface DashboardStats {
  totalUsers: number;
  totalBooks: number;
  activeLoans: number;
  pendingReservations: number;
  monthlyVisits: number;
  revenue: number;
}

interface RecentActivity {
  id: string;
  type: 'loan' | 'return' | 'reservation' | 'user_registration';
  title: string;
  description: string;
  timestamp: Date;
  user: string;
}

@Component({
  selector: 'app-admin-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    AnalyticsDashboardComponent,
    ChartComponent, 
    TypographyComponent,
    ButtonComponent,
    UserAvatarComponent,
    HeadingComponent
  ],
  templateUrl: './admin-dashboard-page.component.html'
})
export class AdminDashboardPageComponent implements OnInit {
  stats: DashboardStats = {
    totalUsers: 0,
    totalBooks: 0,
    activeLoans: 0,
    pendingReservations: 0,
    monthlyVisits: 0,
    revenue: 0
  };

  recentActivities: RecentActivity[] = [];
  loading = true;
  error: string | null = null;

  // Données simulées pour les graphiques
  userGrowthData = {
    labels: ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun'],
    datasets: [
      {
        label: 'Nouveaux utilisateurs',
        data: [65, 59, 80, 81, 56, 55],
        backgroundColor: 'rgba(54, 162, 235, 0.2)',
        borderColor: 'rgba(54, 162, 235, 1)',
        borderWidth: 1
      }
    ]
  };

  bookUsageData = {
    labels: ['Fiction', 'Science', 'Histoire', 'Art', 'Technologie'],
    datasets: [
      {
        label: 'Emprunts par catégorie',
        data: [12, 19, 3, 5, 2],
        backgroundColor: 'rgba(54, 162, 235, 0.2)',
        borderColor: 'rgba(54, 162, 235, 1)',
        borderWidth: 1
      }
    ]
  };

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.error = null;

    // Simulation de chargement des données
    setTimeout(() => {
      try {
        this.stats = {
          totalUsers: 1247,
          totalBooks: 8563,
          activeLoans: 342,
          pendingReservations: 28,
          monthlyVisits: 2456,
          revenue: 12540
        };

        this.recentActivities = [
          {
            id: '1',
            type: 'loan',
            title: 'Nouvel emprunt',
            description: 'Les Misérables emprunté',
            timestamp: new Date(),
            user: 'Jean Dupont'
          },
          {
            id: '2',
            type: 'return',
            title: 'Livre retourné',
            description: '1984 retourné',
            timestamp: new Date(Date.now() - 3600000),
            user: 'Marie Martin'
          },
          {
            id: '3',
            type: 'user_registration',
            title: 'Nouvel utilisateur',
            description: 'Inscription réussie',
            timestamp: new Date(Date.now() - 7200000),
            user: 'Pierre Lambert'
          },
          {
            id: '4',
            type: 'reservation',
            title: 'Nouvelle réservation',
            description: 'Dune réservé',
            timestamp: new Date(Date.now() - 10800000),
            user: 'Sophie Bernard'
          }
        ];

        this.loading = false;
      } catch (err) {
        this.error = 'Erreur lors du chargement des données du tableau de bord';
        this.loading = false;
      }
    }, 1000);
  }

  getActivityIcon(type: string): string {
    switch (type) {
      case 'loan': return 'fa-book';
      case 'return': return 'fa-arrow-left';
      case 'reservation': return 'fa-calendar';
      case 'user_registration': return 'fa-user-plus';
      default: return 'fa-circle';
    }
  }

  getActivityColor(type: string): string {
    switch (type) {
      case 'loan': return 'primary';
      case 'return': return 'success';
      case 'reservation': return 'warning';
      case 'user_registration': return 'info';
      default: return 'secondary';
    }
  }

  formatNumber(value: number): string {
    return value.toLocaleString('fr-FR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: 'EUR'
    }).format(value);
  }

  refreshData(): void {
    this.loadDashboardData();
  }

  navigateToSection(section: string): void {
    // Navigation vers différentes sections de l'admin
    console.log('Navigation vers:', section);
  }
}