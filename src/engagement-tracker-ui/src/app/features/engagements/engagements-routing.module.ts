import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EngagementListComponent } from './engagement-list/engagement-list.component';
import { EngagementDetailComponent } from './engagement-detail/engagement-detail.component';

const routes: Routes = [
  { path: '', component: EngagementListComponent },
  { path: ':id', component: EngagementDetailComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EngagementsRoutingModule {}
