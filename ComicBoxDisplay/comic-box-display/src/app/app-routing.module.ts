import { RouterModule, Routes } from '@angular/router';

import { NgModule } from '@angular/core';
import { BookComponent } from './book/book.component';
import { ChapterComponent } from './book/chapter/chapter.component';
import { PageComponent } from './book/chapter/page/page.component';
import { ModalComponent } from './book/chapter/page/modal/modal.component';

const routes: Routes = [
    { path: ':book/:chapter', component: PageComponent },
    { path: ':book', component: ChapterComponent },
    { path: '', component: BookComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    entryComponents: [ModalComponent]
})
export class AppRoutingModule { }
