import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { TypographyComponent } from '../../../../../app/shared/components/atoms/typography/typography.component';
import { IconComponent } from '../../../../../app/shared/components/atoms/icons/icon.component';
import { HeadingComponent } from '../../../../../app/shared/components/atoms/heading/heading.component';

export type ChartType = 'bar' | 'line' | 'pie' | 'doughnut';

export interface ChartDataset {
  label: string;
  data: number[];
  backgroundColor?: string;
  borderColor?: string;
  borderWidth?: number;
}

export interface ChartData {
  labels: string[];
  datasets: ChartDataset[];
}

@Component({
  selector: 'app-chart',
  imports: [CommonModule, TypographyComponent, IconComponent, HeadingComponent],
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChartComponent implements OnChanges {
  @Input() type: ChartType = 'bar';
  @Input() data: ChartData | null = null;
  @Input() loading = false;
  @Input() title: string | null = null;
  @Input() height = '300px';

  // Couleurs cohérentes avec le design system
  private readonly colorPalette = [
    '#1e3a8a', // primary
    '#8b5cf6', // secondary
    '#fbbf24', // accent
    '#0d9488', // success
    '#dc2626', // danger
    '#0ea5e9'  // info
  ];

  chartSvg: string = '';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data'] && this.data) {
      // Vérifier si les données ont réellement changé pour éviter les mises à jour inutiles
      const newData = JSON.stringify(this.data);
      const oldData = changes['data'].previousValue ? JSON.stringify(changes['data'].previousValue) : null;
      
      if (newData !== oldData) {
        this.generateStaticChart();
      }
    }
  }

  private generateStaticChart(): void {
    if (!this.data || this.data.datasets.length === 0) {
      this.chartSvg = '';
      return;
    }

    const width = 400;
    const height = 300;
    const padding = 40;
    const chartWidth = width - 2 * padding;
    const chartHeight = height - 2 * padding;

    const datasets = this.data.datasets;
    const labels = this.data.labels;
    
    if (this.type === 'bar') {
      this.chartSvg = this.generateBarChart(datasets, labels, width, height, chartWidth, chartHeight, padding);
    } else if (this.type === 'line') {
      this.chartSvg = this.generateLineChart(datasets, labels, width, height, chartWidth, chartHeight, padding);
    } else {
      // Pour pie et doughnut, afficher un message simple
      this.chartSvg = this.generateSimpleChart();
    }
  }

  private generateBarChart(datasets: ChartDataset[], labels: string[], width: number, height: number, chartWidth: number, chartHeight: number, padding: number): string {
    const maxValue = Math.max(...datasets.flatMap(d => d.data));
    const barWidth = chartWidth / labels.length / datasets.length;
    
    let svg = `<svg width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">`;
    
    // Grille de fond
    for (let i = 0; i <= 5; i++) {
      const y = padding + (chartHeight / 5) * i;
      svg += `<line x1="${padding}" y1="${y}" x2="${width - padding}" y2="${y}" stroke="#f1f5f9" stroke-width="1"/>`;
    }

    // Barres
    datasets.forEach((dataset, datasetIndex) => {
      dataset.data.forEach((value, index) => {
        const barHeight = (value / maxValue) * chartHeight;
        const x = padding + index * (chartWidth / labels.length) + datasetIndex * barWidth;
        const y = height - padding - barHeight;
        
        const color = dataset.backgroundColor || this.colorPalette[datasetIndex % this.colorPalette.length];
        svg += `<rect x="${x}" y="${y}" width="${barWidth * 0.8}" height="${barHeight}" fill="${color}" rx="2"/>`;
      });
    });

    // Labels
    labels.forEach((label, index) => {
      const x = padding + (index + 0.5) * (chartWidth / labels.length);
      svg += `<text x="${x}" y="${height - padding + 15}" text-anchor="middle" font-family="Poppins" font-size="10" fill="#64748b">${label}</text>`;
    });

    svg += '</svg>';
    return svg;
  }

  private generateLineChart(datasets: ChartDataset[], labels: string[], width: number, height: number, chartWidth: number, chartHeight: number, padding: number): string {
    const maxValue = Math.max(...datasets.flatMap(d => d.data));
    
    let svg = `<svg width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">`;
    
    // Grille de fond
    for (let i = 0; i <= 5; i++) {
      const y = padding + (chartHeight / 5) * i;
      svg += `<line x1="${padding}" y1="${y}" x2="${width - padding}" y2="${y}" stroke="#f1f5f9" stroke-width="1"/>`;
    }

    // Lignes
    datasets.forEach((dataset, datasetIndex) => {
      const points = dataset.data.map((value, index) => {
        const x = padding + (index / (labels.length - 1)) * chartWidth;
        const y = height - padding - (value / maxValue) * chartHeight;
        return `${x},${y}`;
      }).join(' ');

      const color = dataset.borderColor || this.colorPalette[datasetIndex % this.colorPalette.length];
      svg += `<polyline points="${points}" fill="none" stroke="${color}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>`;
    });

    // Labels
    labels.forEach((label, index) => {
      const x = padding + (index / (labels.length - 1)) * chartWidth;
      svg += `<text x="${x}" y="${height - padding + 15}" text-anchor="middle" font-family="Poppins" font-size="10" fill="#64748b">${label}</text>`;
    });

    svg += '</svg>';
    return svg;
  }

  private generateSimpleChart(): string {
    return `<svg width="400" height="300" viewBox="0 0 400 300">
      <text x="200" y="150" text-anchor="middle" font-family="Poppins" font-size="14" fill="#64748b">
        Graphique ${this.type} - Version statique
      </text>
    </svg>`;
  }
}