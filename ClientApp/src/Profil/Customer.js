import React,{Component} from 'react';
import { Button, ButtonToolbar, Table } from 'reactstrap';
import authService from '../components/api-authorization/AuthorizeService';
export class Customer extends Component{
    constructor(props)
    {
        super(props);
        this.state={
            CustomerID: 1,
            showReturned: false,
            ReturnedCars: [], 
            showCurrentrent:  false, 
            currentRented: [],
        }
    }

    async dowlandCurentRentedCars() {
        const token = await authService.getAccessToken();
        const user = await authService.getUser();
        if(this.state.showCurrentrent)
        {
            fetch(process.env.REACT_APP_API + '/CarApiPrivate/GetRentedCars/'+user.name, {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            })
            .then(response=>response.json())
            .then(data=>{
                this.setState({ currentRented: data });
            });
        }
    }

    async dowlandReturnetCars(){
        if(this.state.showReturned)
        {
            const token = await authService.getAccessToken();
            const user = await authService.getUser();
            fetch(process.env.REACT_APP_API + '/CarApiPrivate/GetReturnedCar/'+user.name, {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            })
            .then(response=>response.json())
            .then(data=>{
                this.setState({ReturnedCars:data});
            });
        }
    }

    refresh(){
        this.dowlandReturnetCars();
        this.dowlandCurentRentedCars();
    }
    

    async return(rentID) {
        const token = await authService.getAccessToken();
        const user = await authService.getUser();
        fetch(process.env.REACT_APP_API + '/CarApiPrivate/Return/' + rentID, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        })
            .then(response=>response.json())
            .then(data=>{
                alert(data);
            });
    }


    componentDidMount(){
        this.refresh();
        this.interval = setInterval(() => this.refresh(), 2000);

    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }


    render(){
        const {currentRented, ReturnedCars} = this.state;
        return(
        <div>
            <ButtonToolbar>
                <Button className="history" variant="info" 
                onClick={()=>{
                    this.setState({showReturned:!this.state.showReturned});
                    
                }}>
                    Wyświetl historię wypożyczeń aut
                </Button>
                <Button className="current" variant="info" 
                onClick={()=>{
                    this.setState({showCurrentrent:!this.state.showCurrentrent});
                    
                    }}>
                    Wyświetl obecnie wypożyczone auta
                </Button>
            </ButtonToolbar>
            <div>
            { this.state.showCurrentrent &&
                <div>
                    <label aria-setsize={40} color="violet"> Obecnie wyporzyczone</label>
                    <Table>
                        <thead>
                        <tr>
                            <th>Marka auta</th>
                            <th>Model auta</th>
                            <th>Id auta</th>
                                    <th>Data wypozyczenia</th>
                                    <th>Przewidywana data zwrotu</th>
                        </tr>
                        </thead>
                        <tbody>
                        {currentRented.map(cr=>
                            <tr key={cr.rentToken}>
                                <td>{cr.carBrand}</td>
                                <td>{cr.carModel}</td>
                                <td>{cr.carDetailsID}</td>
                                <td>{cr.rentDate}</td>
                                <td>{cr.expedtedReturnDate}</td>
                            </tr>
                            )
                        }
                        </tbody>
                    </Table>
                </div>
                }
            </div>
            <div>
                { this.state.showReturned &&
                <div>
                    <label aria-setsize={40} color="violet"> Zwrócone</label>
                    <Table>
                        <thead>
                        <tr>
                            <th>Marka</th>
                            <th>Model</th>
                            <th>Data zwrotu</th>
                        </tr>
                        </thead>
                        <tbody>
                        {ReturnedCars.map(rc=>
                            <tr key={rc.rentToken}>
                                <td>{rc.carBrand}</td>
                                <td>{rc.carModel}</td>
                                <td>{rc.rentDate}</td>
                                <td>
                                    <ButtonToolbar>
                                        <Button>
                                            Szczegóły
                                        </Button>
                                    </ButtonToolbar>
                                </td>
                            </tr>
                            )
                        }
                        </tbody>
                    </Table>
                </div>
                }
            </div>
        </div>
        )
    }
}