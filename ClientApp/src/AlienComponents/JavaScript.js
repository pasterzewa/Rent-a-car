import React, { Component } from 'react';
import { Table, Button, ButtonToolbar } from 'reactstrap';
import CheckPriceForm from './Forms/CheckPriceForm';
import RentMeForm from './Forms/RentMeForm';
import authService from './components/api-authorization/AuthorizeService';
export default class AlienCompanysCars extends Component {

    constructor(props) {
        super(props);
        this.state = {
            details: [],
            checkPrice: false,
            PriceShower: [],
            Rent: false

        };
        this.tryToGetDataFromUser = this.tryToGetDataFromUser.bind(this);
        this.checkPriceClicked = this.checkPriceClicked.bind(this);
        this.additem = this.additem.bind(this);
        this.isinlist = this.additem.bind(this);
        this.removeItem = this.additem.bind(this);
    }


    show_price(i) {
        var price = -1;
        this.state.PriceShower.map(s => {
            if (s.Key === i) {
                price = s.Price;
            }
        });
        if (price === -1)
            return null;
        else
            return price;
    }
    additem(id, price) {
        this.setState(state => {
            var item = { Key: id, Price: price };
            const PriceShower = state.PriceShower.concat(item);

            return {
                PriceShower
            };
        });
    }

    isinlist(i) {
        var should_be_show = false;
        this.state.PriceShower.map(s => {
            if (s.Key === i) {
                should_be_show = true;
            }
        });
        return should_be_show;
    };



    removeItem(i) {
        this.setState(state => {
            const PriceShower = state.PriceShower.filter(item => item.Key !== i);

            return {
                PriceShower,
            };
        });
    };
    async checkPriceClicked(carDetailsID) {
        const token = await authService.getAccessToken();
        const user = await authService.getUser();
        if (!!user) {
            this.tryToGetDataFromUser(carDetailsID, token, user)
        } else {
            this.setState({ checkPrice: true })
        }

    }

    //to jest pojebane ale mam wyjebane
    async tryToGetDataFromUser(carDetailsID, token, user) {
        fetch(process.env.REACT_APP_API + '/CarApiPrivate/CheckUserData/' + user.name, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        })
            .then(response => response.json())
            .then(data => {
                if (!!data) {
                    fetch(process.env.REACT_APP_API + '/CarApi/GetPrice', {
                        method: 'Post',
                        headers: {
                            'Accept': 'application/json',
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            Name: data.Name,
                            Surname: data.Surname,
                            DateofBecomingDriver: data.BecoamingDriverDate.value,
                            Birthday: data.BirtheDate.value,
                            City: data.City.value,
                            Street: data.Street.value,
                            StreetNumber: data.StreetNumber.value,
                            CarDetalisID: carDetailsID
                        })
                    }).then(response2 => response2.json())
                        .then(data2 => {
                            if (this.isinlist(carDetailsID)) {
                                this.removeitem(carDetailsID);
                            }

                            this.additem(carDetailsID, data2);

                        });

                } else {
                    this.setState({ checkPrice: true })
                }


            }).catch(error => this.setState({ checkPrice: true }));


    }


    render() {

        if (this.props.show) {
            const { details } = this.props.;

            let ModalCheckPriceClose = () => this.setState({ checkPrice: false });
            let ModalRentClose = () => this.setState({ Rent: false });

            return (
                <Table className="mt-4" striped bordered hover size="sm">
                    <thead>
                        <tr>
                            <th>Rok Produkcji</th>
                            <th>Opis</th>
                            <th>Cena za dzień</th>
                            <th>Opcje</th>
                        </tr>
                    </thead>
                    <tbody>
                        {details.map(detal =>
                            <tr key={detal.CarDetailsID}>
                                <td>{detal.YearOfProduction}</td>
                                <td>{detal.Description}</td>
                                <td>{this.show_price(detal.CarDetailsID)}</td>
                                <td>
                                    <ButtonToolbar>
                                        <Button className="mr-2" variant="info"
                                            onClick={() => this.checkPriceClicked(detal.CarDetailsID)}>
                                            Sprawdź cene
                                        </Button>


                                        {
                                            this.show_price(detal.CarDetailsID) != null &&
                                            <Button className="mr-2" variant="success"
                                                onClick={() => this.setState({ Rent: detal.CarDetailsID })}>
                                                Wynajmij mnie
                                            </Button>
                                        }


                                        <CheckPriceForm isOpen={this.state.checkPrice}
                                            onHide={ModalCheckPriceClose}
                                            cardetalisid={detal.CarDetailsID}
                                            isinlist={this.isinlist}
                                            removeitem={this.removeItem}
                                            additem={this.additem} />

                                        <RentMeForm isOpen={this.state.Rent}
                                            onHide={ModalRentClose}
                                            carDetailsID={detal.CarDetailsID} />
                                    </ButtonToolbar>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </Table>

            )
        }
        else
            return (null)
    }
}

