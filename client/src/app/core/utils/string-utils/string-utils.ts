// Utilitaires de chaînes de caractères

export class StringUtils {
  // Capitaliser la première lettre d'une chaîne
  static capitalize(str: string): string {
    if (!str) return str;
    return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
  }

  // Capitaliser chaque mot d'une chaîne
  static capitalizeWords(str: string): string {
    if (!str) return str;
    return str
      .split(' ')
      .map(word => this.capitalize(word))
      .join(' ');
  }

  // Formater un nom propre (capitalisation intelligente)
  static formatProperName(str: string): string {
    if (!str) return str;
    
    // Liste des particules à ne pas capitaliser
    const particles = ['de', 'du', 'des', 'le', 'la', 'les', 'd', 'l'];
    
    return str
      .toLowerCase()
      .split(' ')
      .map((word, index) => {
        // Ne pas capitaliser les particules sauf en première position
        if (index > 0 && particles.includes(word)) {
          return word;
        }
        return this.capitalize(word);
      })
      .join(' ');
  }

  // Tronquer une chaîne avec des points de suspension
  static truncate(str: string, maxLength: number, suffix: string = '...'): string {
    if (!str || str.length <= maxLength) return str;
    return str.substring(0, maxLength - suffix.length) + suffix;
  }

  // Tronquer une chaîne en préservant les mots complets
  static truncateWords(str: string, maxLength: number, suffix: string = '...'): string {
    if (!str || str.length <= maxLength) return str;
    
    const truncated = str.substring(0, maxLength - suffix.length);
    const lastSpace = truncated.lastIndexOf(' ');
    
    if (lastSpace > 0) {
      return truncated.substring(0, lastSpace) + suffix;
    }
    
    return truncated + suffix;
  }

  // Supprimer les accents d'une chaîne
  static removeAccents(str: string): string {
    if (!str) return str;
    return str.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
  }

  // Formater un numéro de téléphone français
  static formatFrenchPhone(phone: string): string {
    if (!phone) return phone;
    
    const cleaned = phone.replace(/\D/g, '');
    
    if (cleaned.length === 10) {
      return cleaned.replace(/(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})/, '$1 $2 $3 $4 $5');
    }
    
    return phone;
  }

  // Formater un code postal français
  static formatFrenchPostalCode(postalCode: string): string {
    if (!postalCode) return postalCode;
    
    const cleaned = postalCode.replace(/\D/g, '');
    
    if (cleaned.length === 5) {
      return cleaned;
    }
    
    return postalCode;
  }

  // Vérifier si une chaîne est un email valide
  static isValidEmail(email: string): boolean {
    if (!email) return false;
    
    const emailRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;
    return emailRegex.test(email);
  }

  // Vérifier si une chaîne est un URL valide
  static isValidUrl(url: string): boolean {
    if (!url) return false;
    
    try {
      new URL(url);
      return true;
    } catch {
      return false;
    }
  }

  // Extraire le domaine d'un URL
  static extractDomain(url: string): string {
    if (!this.isValidUrl(url)) return '';
    
    try {
      const urlObj = new URL(url);
      return urlObj.hostname;
    } catch {
      return '';
    }
  }

  // Nettoyer une chaîne pour la recherche (suppression accents, minuscules)
  static cleanForSearch(str: string): string {
    if (!str) return str;
    return this.removeAccents(str).toLowerCase().trim();
  }

  // Générer un slug à partir d'une chaîne
  static generateSlug(str: string): string {
    if (!str) return '';
    
    return str
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/[^a-z0-9 -]/g, '')
      .replace(/\s+/g, '-')
      .replace(/-+/g, '-')
      .trim();
  }

  // Générer un identifiant unique simple
  static generateId(length: number = 8): string {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    
    for (let i = 0; i < length; i++) {
      result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    
    return result;
  }

  // Formater un nombre avec séparateurs de milliers
  static formatNumber(number: number, locale: string = 'fr-FR'): string {
    return new Intl.NumberFormat(locale).format(number);
  }

  // Formater une taille de fichier en octets en format lisible
  static formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 octets';
    
    const sizes = ['octets', 'Ko', 'Mo', 'Go', 'To'];
    const i = Math.floor(Math.log(bytes) / Math.log(1024));
    
    return Math.round(bytes / Math.pow(1024, i) * 100) / 100 + ' ' + sizes[i];
  }

  // Masquer partiellement un email (ex: te***@example.com)
  static maskEmail(email: string): string {
    if (!this.isValidEmail(email)) return email;
    
    const [localPart, domain] = email.split('@');
    const maskedLocal = localPart.substring(0, 2) + '***';
    
    return maskedLocal + '@' + domain;
  }

  // Masquer partiellement un numéro de téléphone
  static maskPhone(phone: string): string {
    if (!phone) return phone;
    
    const cleaned = phone.replace(/\D/g, '');
    
    if (cleaned.length === 10) {
      return '** ** ** ' + cleaned.substring(6);
    }
    
    return phone;
  }

  // Extraire les initiales d'un nom complet
  static getInitials(fullName: string): string {
    if (!fullName) return '';
    
    return fullName
      .split(' ')
      .map(word => word.charAt(0).toUpperCase())
      .join('')
      .substring(0, 2);
  }

  // Compter les mots dans une chaîne
  static wordCount(str: string): number {
    if (!str) return 0;
    return str.trim().split(/\s+/).length;
  }

  // Compter les caractères sans espaces
  static characterCount(str: string): number {
    if (!str) return 0;
    return str.replace(/\s/g, '').length;
  }

  // Vérifier si une chaîne contient uniquement des lettres
  static isAlpha(str: string): boolean {
    if (!str) return false;
    return /^[A-Za-zÀ-ÿ\s]+$/.test(str);
  }

  // Vérifier si une chaîne contient uniquement des chiffres
  static isNumeric(str: string): boolean {
    if (!str) return false;
    return /^\d+$/.test(str);
  }

  // Vérifier si une chaîne est alphanumérique
  static isAlphaNumeric(str: string): boolean {
    if (!str) return false;
    return /^[A-Za-zÀ-ÿ0-9\s]+$/.test(str);
  }

  // Supprimer les balises HTML d'une chaîne
  static stripHtml(html: string): string {
    if (!html) return html;
    return html.replace(/<[^>]*>/g, '');
  }

  // Échapper les caractères spéciaux HTML
  static escapeHtml(unsafe: string): string {
    if (!unsafe) return unsafe;
    
    return unsafe
      .replace(/&/g, '&')
      .replace(/</g, '<')
      .replace(/>/g, '>')
      .replace(/"/g, '"')
      .replace(/'/g, '&#039;');
  }

  // Déséchapper les caractères HTML
  static unescapeHtml(safe: string): string {
    if (!safe) return safe;
    
    return safe
      .replace(/&/g, '&')
      .replace(/</g, '<')
      .replace(/>/g, '>')
      .replace(/"/g, '"')
      .replace(/&#039;/g, "'");
  }

  // Formater une durée en secondes en format MM:SS
  static formatDuration(seconds: number): string {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  // Convertir une chaîne camelCase en texte lisible
  static camelCaseToReadable(str: string): string {
    if (!str) return str;
    
    return str
      .replace(/([A-Z])/g, ' $1')
      .replace(/^./, char => char.toUpperCase())
      .trim();
  }

  // Convertir une chaîne snake_case en texte lisible
  static snakeCaseToReadable(str: string): string {
    if (!str) return str;
    
    return str
      .split('_')
      .map(word => this.capitalize(word))
      .join(' ');
  }

  // Normaliser les espaces multiples
  static normalizeSpaces(str: string): string {
    if (!str) return str;
    return str.replace(/\s+/g, ' ').trim();
  }

  // Vérifier si une chaîne est vide ou ne contient que des espaces
  static isBlank(str: string): boolean {
    return !str || str.trim().length === 0;
  }

  // Comparer deux chaînes pour la recherche (insensible à la casse et aux accents)
  static searchCompare(str1: string, str2: string): boolean {
    if (!str1 || !str2) return false;
    return this.cleanForSearch(str1).includes(this.cleanForSearch(str2));
  }

  // Obtenir les premières lettres de chaque mot
  static getFirstLetters(str: string): string {
    if (!str) return '';
    
    return str
      .split(' ')
      .map(word => word.charAt(0).toUpperCase())
      .join('');
  }

  // Formater un texte pour l'affichage multiligne
  static formatMultiline(text: string, maxLineLength: number = 80): string[] {
    if (!text) return [];
    
    const words = text.split(' ');
    const lines: string[] = [];
    let currentLine = '';
    
    for (const word of words) {
      if ((currentLine + word).length > maxLineLength) {
        if (currentLine) {
          lines.push(currentLine.trim());
          currentLine = word + ' ';
        } else {
          lines.push(word);
        }
      } else {
        currentLine += word + ' ';
      }
    }
    
    if (currentLine.trim()) {
      lines.push(currentLine.trim());
    }
    
    return lines;
  }
}