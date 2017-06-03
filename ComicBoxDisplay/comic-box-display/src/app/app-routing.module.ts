import { RouterModule, Routes } from '@angular/router';

import { NgModule } from '@angular/core';
import { BookComponent } from './book/book.component';
import { ChapterComponent } from './book/chapter/chapter.component';
import { PageComponent } from './book/chapter/page/page.component';

const routes: Routes = [        
    { path: ':book/:chapter', component: ChapterComponent },
    { path: ':book', component: ChapterComponent },
    { path: '', component: BookComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    entryComponents: [PageComponent]
})
export class AppRoutingModule { }
