import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import { UserProvider } from './auth/UserProvider'
import App from './App';
import * as serviceWorkerRegistration from './serviceWorkerRegistration';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
  <UserProvider>
    <BrowserRouter basename={baseUrl}>
      <App />
    </BrowserRouter>
  </UserProvider>,
  rootElement);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
serviceWorkerRegistration.unregister();

