import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { DataCollectionComponent } from './components/datacollection/datacollection.component';
import { ResultsComponent } from './components/results/results.component';

import { AppService } from './app.service';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        DataCollectionComponent,
        ResultsComponent
    ],
    providers: [
        AppService
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'data-collection', pathMatch: 'full' },
            { path: 'data-collection', component: DataCollectionComponent },
            { path: 'results', component: ResultsComponent },
            { path: '**', redirectTo: 'data-collection' }
        ])
    ]
})
export class AppModuleShared {
}
