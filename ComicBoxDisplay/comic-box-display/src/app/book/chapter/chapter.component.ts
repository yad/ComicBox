import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { MdDialog } from '@angular/material';
import { PageComponent } from './page/page.component';

@Component({
    selector: 'app-chapter',
    templateUrl: './chapter.component.html',
    styleUrls: ['./chapter.component.css']
})
export class ChapterComponent implements OnInit {

    public chapters: any[];

    public book: string;

    constructor(private route: ActivatedRoute, private http: Http, private dialog: MdDialog) {
    }

    openDialog(currentChapter: string) {
        this.dialog.open(PageComponent, {
            data: {
                book: this.book,
                chapter: currentChapter
            }
        });
    }

    ngOnInit() {
        this.route.params.subscribe(params => {
            this.book = params["book"];

            this.http.get("/api/book/comics/" + this.book).subscribe(result => {
                this.chapters = result.json().collection.map(chapter => ({
                    name: chapter.name,
                    displayName: "Tome #" + parseInt(chapter.name.slice(0, -4)),
                    thumbnail: 'data:image/png;base64,' + chapter.thumbnail
                }));
            });
        });
    }
}
