import { RouterModule, Routes } from '@angular/router';

import { NgModule } from '@angular/core';
import { BookComponent } from './book/book.component';
import { ChapterComponent } from './book/chapter/chapter.component';
import { PageComponent } from './book/chapter/page/page.component';

const routes: Routes = [
    { path: '', pathMatch: 'full', redirectTo: 'books', },
    { path: 'books', component: BookComponent },
    { path: 'chapters', component: ChapterComponent },
    { path: 'pages', component: PageComponent },
    { path: '**', pathMatch: 'full', redirectTo: 'books' },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
