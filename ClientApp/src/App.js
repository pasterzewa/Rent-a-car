import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import './custom.css'

import  Cars  from './Cars';
import { Customer } from './Profil/Customer'
import { Worker } from './Profil/Worker'
export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
        <Layout>
            <Route exact path='/' component={Cars} />
            <Route path='/Cars' component={Cars}/>
            <Route path='/Profil/Customer' component={Customer} />
            <Route path='/Profil/Worker' component={Worker} />
            <Route path='/counter' component={Counter} />
            <AuthorizeRoute path='/fetch-data' component={FetchData} />
            <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
         
      </Layout>
    );
  }
}
