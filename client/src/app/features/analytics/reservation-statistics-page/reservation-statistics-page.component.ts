import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnalyticsService } from '../../../core/services/api/analytics.service';

interface ReservationStat {
  period: string;
  totalReservations: number;
  activeReservations: number;
  completedReservations: number;
  cancelledReservations: number;
  avgReservationDuration: number;
  conversionRate: number;
}

interface PopularBookReservation {
  bookId: string;
  title: string;
  author: string;
  reservationCount: number;
  category: string;
}

interface ReservationTrend {
  date: string;
  reservations: number;
  completions: number;
  cancellations: number;
}

@Component({
  selector: 'app-reservation-statistics-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reservation-statistics-page.component.html'
})
export class ReservationStatisticsPageComponent implements OnInit {
  // Données de statistiques
  reservationStats: ReservationStat[] = [];
  popularBooks: PopularBookReservation[] = [];
  reservationTrends: ReservationTrend[] = [];
  
  // Filtres
  selectedPeriod: string = '7days';
  selectedCategory: string = 'all';
  
  // Métriques globales
  totalReservations: number = 0;
  activeReservations: number = 0;
  avgCompletionTime: number = 0;
  conversionRate: number = 0;
  
  // États de chargement
  isLoading: boolean = false;
  hasError: boolean = false;

  constructor(private analyticsService: AnalyticsService) {}

  ngOnInit(): void {
    this.loadReservationStatistics();
  }

  loadReservationStatistics(): void {
    this.isLoading = true;
    this.hasError = false;

    // Simulation de données - à remplacer par des appels API réels
    setTimeout(() => {
      try {
        // Générer des données de test
        this.generateMockData();
        this.calculateGlobalMetrics();
        this.isLoading = false;
      } catch (error) {
        this.hasError = true;
        this.isLoading = false;
        console.error('Erreur lors du chargement des statistiques:', error);
      }
    }, 1000);
  }

  private generateMockData(): void {
    // Données de statistiques par période
    this.reservationStats = [
      {
        period: 'Aujourd\'hui',
        totalReservations: 12,
        activeReservations: 8,
        completedReservations: 3,
        cancelledReservations: 1,
        avgReservationDuration: 2.5,
        conversionRate: 25
      },
      {
        period: 'Semaine',
        totalReservations: 84,
        activeReservations: 45,
        completedReservations: 32,
        cancelledReservations: 7,
        avgReservationDuration: 2.8,
        conversionRate: 38
      },
      {
        period: 'Mois',
        totalReservations: 325,
        activeReservations: 156,
        completedReservations: 142,
        cancelledReservations: 27,
        avgReservationDuration: 3.1,
        conversionRate: 44
      }
    ];

    // Livres les plus réservés
    this.popularBooks = [
      {
        bookId: '1',
        title: 'Le Petit Prince',
        author: 'Antoine de Saint-Exupéry',
        reservationCount: 28,
        category: 'Fiction'
      },
      {
        bookId: '2',
        title: '1984',
        author: 'George Orwell',
        reservationCount: 24,
        category: 'Science-Fiction'
      },
      {
        bookId: '3',
        title: 'L\'Étranger',
        author: 'Albert Camus',
        reservationCount: 21,
        category: 'Philosophie'
      },
      {
        bookId: '4',
        title: 'Harry Potter à l\'école des sorciers',
        author: 'J.K. Rowling',
        reservationCount: 19,
        category: 'Fantasy'
      },
      {
        bookId: '5',
        title: 'Le Seigneur des Anneaux',
        author: 'J.R.R. Tolkien',
        reservationCount: 17,
        category: 'Fantasy'
      }
    ];

    // Tendances des réservations (7 derniers jours)
    const dates = this.generateLast7Days();
    this.reservationTrends = dates.map((date, index) => ({
      date,
      reservations: Math.floor(Math.random() * 20) + 5,
      completions: Math.floor(Math.random() * 15) + 3,
      cancellations: Math.floor(Math.random() * 5) + 1
    }));
  }

  private generateLast7Days(): string[] {
    const dates: string[] = [];
    for (let i = 6; i >= 0; i--) {
      const date = new Date();
      date.setDate(date.getDate() - i);
      dates.push(date.toLocaleDateString('fr-FR'));
    }
    return dates;
  }

  private calculateGlobalMetrics(): void {
    const monthlyStats = this.reservationStats.find(stat => stat.period === 'Mois');
    if (monthlyStats) {
      this.totalReservations = monthlyStats.totalReservations;
      this.activeReservations = monthlyStats.activeReservations;
      this.avgCompletionTime = monthlyStats.avgReservationDuration;
      this.conversionRate = monthlyStats.conversionRate;
    }
  }

  onPeriodChange(): void {
    this.loadReservationStatistics();
  }

  onCategoryChange(): void {
    this.loadReservationStatistics();
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'active': return 'bg-blue-100 text-blue-800';
      case 'completed': return 'bg-green-100 text-green-800';
      case 'cancelled': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  getTrendIcon(trend: number): string {
    if (trend > 0) return '📈';
    if (trend < 0) return '📉';
    return '➡️';
  }

  getPopularityColor(count: number): string {
    if (count >= 20) return 'text-red-600 font-bold';
    if (count >= 15) return 'text-orange-600 font-semibold';
    if (count >= 10) return 'text-yellow-600 font-medium';
    return 'text-green-600';
  }
}