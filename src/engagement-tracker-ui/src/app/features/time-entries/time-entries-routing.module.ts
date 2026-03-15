import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TimeEntryListComponent } from './time-entry-list/time-entry-list.component';

const routes: Routes = [{ path: '', component: TimeEntryListComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TimeEntriesRoutingModule {}
