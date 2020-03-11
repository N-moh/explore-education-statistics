import '@admin/polyfill';

import initApplicationInsights from '@admin/services/applicationInsightsService';
import '@common/polyfill';
import React from 'react';
import ReactDOM from 'react-dom';
import * as serviceWorker from './serviceWorker';

process.env.APP_ROOT_ID = 'root';

import('./App').then(({ default: App }) => {
  ReactDOM.render(<App />, document.getElementById('root'));
});

initApplicationInsights();

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
