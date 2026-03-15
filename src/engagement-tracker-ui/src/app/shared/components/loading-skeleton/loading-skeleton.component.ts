import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-loading-skeleton',
  templateUrl: './loading-skeleton.component.html',
  styleUrls: ['./loading-skeleton.component.scss']
})
export class LoadingSkeletonComponent {
  @Input() rows = 5;
  @Input() type: 'table' | 'cards' = 'table';

  get rowArray(): number[] {
    return Array.from({ length: this.rows }, (_, i) => i);
  }
}
