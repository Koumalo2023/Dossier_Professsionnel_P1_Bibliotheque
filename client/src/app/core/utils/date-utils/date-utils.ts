// Utilitaires de dates

export class DateUtils {
  // Formater une date en français
  static formatDate(date: Date | string, format: 'short' | 'medium' | 'long' = 'medium'): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: format === 'short' ? '2-digit' : format === 'medium' ? 'short' : 'long',
      day: '2-digit',
    };
    
    return dateObj.toLocaleDateString('fr-FR', options);
  }

  // Formater une date avec l'heure
  static formatDateTime(date: Date | string, includeSeconds: boolean = false): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: includeSeconds ? '2-digit' : undefined,
    };
    
    return dateObj.toLocaleDateString('fr-FR', options);
  }

  // Formater une durée en minutes en texte lisible
  static formatDuration(minutes: number): string {
    if (minutes < 60) {
      return `${minutes} min`;
    }
    
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    
    if (remainingMinutes === 0) {
      return `${hours} h`;
    }
    
    return `${hours} h ${remainingMinutes} min`;
  }

  // Formater une date relative (il y a...)
  static formatRelativeDate(date: Date | string): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const now = new Date();
    const diffMs = now.getTime() - dateObj.getTime();
    const diffSeconds = Math.floor(diffMs / 1000);
    const diffMinutes = Math.floor(diffSeconds / 60);
    const diffHours = Math.floor(diffMinutes / 60);
    const diffDays = Math.floor(diffHours / 24);
    
    if (diffSeconds < 60) {
      return 'à l\'instant';
    } else if (diffMinutes < 60) {
      return `il y a ${diffMinutes} min`;
    } else if (diffHours < 24) {
      return `il y a ${diffHours} h`;
    } else if (diffDays < 7) {
      return `il y a ${diffDays} j`;
    } else if (diffDays < 30) {
      const weeks = Math.floor(diffDays / 7);
      return `il y a ${weeks} sem`;
    } else if (diffDays < 365) {
      const months = Math.floor(diffDays / 30);
      return `il y a ${months} mois`;
    } else {
      const years = Math.floor(diffDays / 365);
      return `il y a ${years} an${years > 1 ? 's' : ''}`;
    }
  }

  // Calculer l'âge à partir d'une date de naissance
  static calculateAge(birthDate: Date | string): number {
    const birthDateObj = typeof birthDate === 'string' ? new Date(birthDate) : birthDate;
    const today = new Date();
    let age = today.getFullYear() - birthDateObj.getFullYear();
    const monthDiff = today.getMonth() - birthDateObj.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDateObj.getDate())) {
      age--;
    }
    
    return age;
  }

  // Vérifier si une date est aujourd'hui
  static isToday(date: Date | string): boolean {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const today = new Date();
    
    return dateObj.toDateString() === today.toDateString();
  }

  // Vérifier si une date est dans le futur
  static isFuture(date: Date | string): boolean {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj > new Date();
  }

  // Vérifier si une date est dans le passé
  static isPast(date: Date | string): boolean {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj < new Date();
  }

  // Ajouter des jours à une date
  static addDays(date: Date | string, days: number): Date {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const result = new Date(dateObj);
    result.setDate(result.getDate() + days);
    return result;
  }

  // Ajouter des mois à une date
  static addMonths(date: Date | string, months: number): Date {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const result = new Date(dateObj);
    result.setMonth(result.getMonth() + months);
    return result;
  }

  // Ajouter des années à une date
  static addYears(date: Date | string, years: number): Date {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const result = new Date(dateObj);
    result.setFullYear(result.getFullYear() + years);
    return result;
  }

  // Calculer la différence en jours entre deux dates
  static daysBetween(date1: Date | string, date2: Date | string): number {
    const date1Obj = typeof date1 === 'string' ? new Date(date1) : date1;
    const date2Obj = typeof date2 === 'string' ? new Date(date2) : date2;
    
    const diffMs = Math.abs(date2Obj.getTime() - date1Obj.getTime());
    return Math.floor(diffMs / (1000 * 60 * 60 * 24));
  }

  // Obtenir le premier jour du mois
  static getFirstDayOfMonth(date: Date | string): Date {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return new Date(dateObj.getFullYear(), dateObj.getMonth(), 1);
  }

  // Obtenir le dernier jour du mois
  static getLastDayOfMonth(date: Date | string): Date {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return new Date(dateObj.getFullYear(), dateObj.getMonth() + 1, 0);
  }

  // Vérifier si deux dates sont le même jour
  static isSameDay(date1: Date | string, date2: Date | string): boolean {
    const date1Obj = typeof date1 === 'string' ? new Date(date1) : date1;
    const date2Obj = typeof date2 === 'string' ? new Date(date2) : date2;
    
    return date1Obj.toDateString() === date2Obj.toDateString();
  }

  // Formater une date pour l'input date HTML
  static toDateInputValue(date: Date | string): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const local = new Date(dateObj);
    local.setMinutes(dateObj.getMinutes() - dateObj.getTimezoneOffset());
    return local.toJSON().slice(0, 10);
  }

  // Obtenir le nom du mois en français
  static getMonthName(month: number, format: 'short' | 'long' = 'long'): string {
    const date = new Date();
    date.setMonth(month);
    
    return date.toLocaleDateString('fr-FR', { 
      month: format 
    });
  }

  // Obtenir le nom du jour de la semaine en français
  static getDayName(day: number, format: 'short' | 'long' = 'long'): string {
    const date = new Date();
    // On part du dimanche (0) et on ajoute le jour spécifié
    date.setDate(date.getDate() - date.getDay() + day);
    
    return date.toLocaleDateString('fr-FR', { 
      weekday: format 
    });
  }

  // Générer un tableau de dates pour un mois donné
  static getDaysInMonth(year: number, month: number): Date[] {
    const date = new Date(year, month, 1);
    const days: Date[] = [];
    
    while (date.getMonth() === month) {
      days.push(new Date(date));
      date.setDate(date.getDate() + 1);
    }
    
    return days;
  }

  // Vérifier si une date est un jour de week-end
  static isWeekend(date: Date | string): boolean {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const day = dateObj.getDay();
    return day === 0 || day === 6; // 0 = dimanche, 6 = samedi
  }

  // Obtenir le prochain jour ouvré (lundi-vendredi)
  static getNextBusinessDay(date: Date | string): Date {
    let dateObj = typeof date === 'string' ? new Date(date) : date;
    dateObj = this.addDays(dateObj, 1);
    
    while (this.isWeekend(dateObj)) {
      dateObj = this.addDays(dateObj, 1);
    }
    
    return dateObj;
  }

  // Formater une période (date de début et fin)
  static formatPeriod(startDate: Date | string, endDate: Date | string): string {
    const start = typeof startDate === 'string' ? new Date(startDate) : startDate;
    const end = typeof endDate === 'string' ? new Date(endDate) : endDate;
    
    if (this.isSameDay(start, end)) {
      return this.formatDate(start);
    }
    
    if (start.getMonth() === end.getMonth() && start.getFullYear() === end.getFullYear()) {
      return `${start.getDate()} - ${this.formatDate(end)}`;
    }
    
    if (start.getFullYear() === end.getFullYear()) {
      return `${this.formatDate(start, 'short')} - ${this.formatDate(end, 'short')}`;
    }
    
    return `${this.formatDate(start)} - ${this.formatDate(end)}`;
  }
}