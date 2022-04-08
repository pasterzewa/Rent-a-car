import React, { Component, Fragment } from 'react';
import { NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import authService from './AuthorizeService';
import { ApplicationPaths } from './ApiAuthorizationConstants';

export class LoginMenu extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isAuthenticated: false,
            userName: null,
            accountType: null
        };
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    async populateState() {
        const [isAuthenticated, user, account] = await Promise.all([authService.isAuthenticated(), authService.getUser(), authService.getAccountType()])
        this.setState({
            isAuthenticated,
            userName: user && user.name,
            accountType: account
        });
    }

    render() {
        const { isAuthenticated, userName } = this.state;
        if (!isAuthenticated) {
            const registerPath = `${ApplicationPaths.Register}`;
            const loginPath = `${ApplicationPaths.Login}`;
            return this.anonymousView(registerPath, loginPath);
        } else {
            const profilePath = `${ApplicationPaths.Profile}`;
            const logoutPath = { pathname: `${ApplicationPaths.LogOut}`, state: { local: true } };
            return this.authenticatedView(userName, profilePath, logoutPath);
        }
    }

    authenticatedView(userName, profilePath, logoutPath) {
        if (!!this.state.accountType && this.state.accountType === 1) {
            return (<Fragment>
                <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/Profil/Worker">Profil Pracownika</NavLink>
                </NavItem>
                <NavItem>
                    <NavLink tag={Link} className="text-dark" to={profilePath}>Witaj {userName}</NavLink>
                </NavItem>
                <NavItem>
                    <NavLink tag={Link} className="text-dark" to={logoutPath}>Wyloguj</NavLink>
                </NavItem>
            </Fragment>);
        } else {
            return (<Fragment>
                <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/Profil/Customer">Profil Klienta</NavLink>
                </NavItem>
                <NavItem>
                    <NavLink tag={Link} className="text-dark" to={profilePath}>Witaj {userName}</NavLink>
                </NavItem>
                <NavItem>
                    <NavLink tag={Link} className="text-dark" to={logoutPath}>Wyloguj</NavLink>
                </NavItem>
            </Fragment>);
        }
    }

    anonymousView(registerPath, loginPath) {
        return (<Fragment>
            <NavItem>
                <NavLink tag={Link} className="text-dark" to={registerPath}>Zarejestruj sie</NavLink>
            </NavItem>
            <NavItem>
                <NavLink tag={Link} className="text-primary" to={loginPath}>Zaloguj</NavLink>
            </NavItem>
        </Fragment>);
    }
}
