import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'hours', standalone: false })
export class HoursPipe implements PipeTransform {
  transform(value: number): string {
    if (value == null) return '';
    const h = Math.floor(value);
    const m = Math.round((value - h) * 60);
    if (m === 0) return `${h}h`;
    if (h === 0) return `${m}m`;
    return `${h}h ${m}m`;
  }
}
