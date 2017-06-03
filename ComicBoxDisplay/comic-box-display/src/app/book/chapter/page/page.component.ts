import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { MD_DIALOG_DATA } from '@angular/material';

@Component({
    selector: 'app-page',
    templateUrl: './page.component.html',
    styleUrls: ['./page.component.css']
})
export class PageComponent implements OnInit {

    public currentPage: any;

    public image: string;

    private book: string;

    private chapter: string;

    constructor( @Inject(MD_DIALOG_DATA) public data: any, private http: Http) {
        this.book = data.book;
        this.chapter = data.chapter;
    }

    changePage($event) {
        const halfScreenPosition = $event.view.innerWidth / 2;
        if ($event.x > halfScreenPosition) {
            this.nextPageOrChapter();
        }
        else {
            this.previousPageOrChapter();
        }        
    }

    ngOnInit() {
        this.callApi(this.book, this.chapter, 1);
    }

    private nextPageOrChapter() {
        const next = this.currentPage.next;
        if (next) {
            this.callApi(this.book, next.chapterNumber, next.pageNumber);
        }
    }

    private previousPageOrChapter() {
        const previous = this.currentPage.previous;
        if (previous) {
            this.callApi(this.book, previous.chapterNumber, previous.pageNumber);
        }
    }

    private callApi(book: string, chapter: string, page: number) {
        this.http.get(`/api/book/comics/${book}/${chapter}/${page}`).subscribe(response => {
            const result = response.json();
            this.currentPage = result;
            this.image = `data:image/png;base64,${result.content}`;

            document.getElementsByTagName('md-dialog-container')[0].scrollTop = 0;
        });
    }

}
