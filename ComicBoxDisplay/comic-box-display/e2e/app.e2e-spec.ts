import { ComicBoxDisplayPage } from './app.po';

describe('comic-box-display App', () => {
  let page: ComicBoxDisplayPage;

  beforeEach(() => {
    page = new ComicBoxDisplayPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
