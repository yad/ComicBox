import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MdDialog } from '@angular/material';
import { ModalComponent } from './modal/modal.component';

@Component({
    selector: 'app-page',
    templateUrl: './page.component.html',
    styleUrls: ['./page.component.css']
})
export class PageComponent implements OnInit {

    constructor(private route: ActivatedRoute, private router: Router, private dialog: MdDialog) {
    }

    ngOnInit() {
        this.route.params.subscribe(params => {
            const book: string = params["book"];
            const chapter: number = params["chapter"];
            this.openDialog(book, chapter);
        });
    }

    openDialog(book: string, chapter: number) {
        this.dialog.open(ModalComponent, {
            data: {
                book: book,
                chapter: chapter
            }
        });

        this.dialog.afterAllClosed.subscribe(() => {
            this.router.navigate(['/', book]);
        });
    }
}
