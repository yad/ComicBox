import { RouterModule, Routes } from '@angular/router';

import { NgModule } from '@angular/core';
import { BookComponent } from './book/book.component';
import { ChapterComponent } from './book/chapter/chapter.component';
import { PageComponent } from './book/chapter/page/page.component';

const routes: Routes = [
    { path: 'books', component: BookComponent },
    { path: 'books/:book/chapters', component: ChapterComponent },
    { path: 'books/:book/chapters/:chapter', component: PageComponent },
    { path: '', pathMatch: 'full', redirectTo: 'books', },
    { path: '**', pathMatch: 'full', redirectTo: 'books' },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
