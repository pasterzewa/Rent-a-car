import React,{Component} from 'react';
import {Table, Button, ButtonToolbar} from 'reactstrap';
import CheckPriceForm from './Forms/CheckPriceForm';
import RentMeForm from './Forms/RentMeForm';
import authService from './components/api-authorization/AuthorizeService';
export default class CompanysCars extends Component {

    constructor(props) {
        super(props);
        this.state = {
            details: [],
            checkPrice: false,
            PriceShower: [],
            Rent: false,
            quotaId: []

        };
        this.tryToGetDataFromUser = this.tryToGetDataFromUser.bind(this);
        this.checkPriceClicked = this.checkPriceClicked.bind(this);
        this.additem = this.additem.bind(this);
        this.isinlist = this.additem.bind(this);
        this.removeItem = this.additem.bind(this);
    }

    refreshList() {
        if (this.props.show) {
            if (this.props.api === "OurApi") {
                fetch(process.env.REACT_APP_API + '/JsonCars/GetCompanysCars/' + this.props.companyID + '?carID=' + this.props.carID)
                    .then(response => response.json())
                    .then(data => {
                        this.setState({ details: data });
                    });
            }
            if (this.props.api === "Alien") {
                this.setState({
                    details: [{
                        CarDetailsID: this.props.messengFromAlien.id,
                        YearOfProduction: this.props.messengFromAlien.year,
                        Description: this.props.messengFromAlien.description
                    }]
                });
            }
        }
    }

    componentDidMount() {
        this.refreshList();
        this.interval = setInterval(() => this.refreshList(), 2000);

    }

    componentWillUnmount() {
        clearInterval(this.interval);
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

    takeQID(i) {
        var qID = null;
        this.state.quotaId.map(s => {
            if (s.Key === i) {
                qID = s.qId;
            }
        });
         return qID;
    }
    additem(id, price, option = 0) {
        
        this.setState(state => {
            if (option === 0) {
                var item = { Key: id, Price: price };
                const PriceShower = state.PriceShower.concat(item);

                return {
                    PriceShower
                };
            }
            else {
                var item = { Key: id, qId: price };
                const quotaId = state.quotaId.concat(item);

                return {
                    quotaId
                };
            }
        });
    }

    isinlist(i, option = 0){
        var should_be_show = false;
        if (option === 0) {
            this.state.PriceShower.map(s => {
                if (s.Key === i) {
                    should_be_show = true;
                }
            });
            return should_be_show;
        }
        else {
            this.state.quotaId.map(s => {
                if (s.Key === i) {
                    should_be_show = true;
                }
            });
            return should_be_show;
        }
    };
    
    
    
    removeItem(i, option = 0) {
        this.setState(state => {
            if (option === 0) {

            
            const PriceShower = state.PriceShower.filter(item => item.Key !== i);
    
            return {
                PriceShower,
                };
            }
            else {


                const quotaId = state.quotaId.filter(item => item.Key !== i);

                return {
                    quotaId,
                };
            }
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
        fetch(process.env.REACT_APP_API + '/CarApiPrivate/CheckUserData/'+user.name, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        })
            .then(response => response.json())
            .then(data => {
                if (!!data) {
                    if (this.props.api === "OurApi") {

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
                    }

                } else if (this.props.api === "Alien") {
                    console.log("kupa");
                    fetch(process.env.REACT_APP_API + '/AlienApi/GetPrice/' + carDetailsID, {
                        method: 'Post',
                        headers: {
                            'Accept': 'application/json',
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            age: new Date().getFullYear() - new Date(data.BirtheDate.value).getFullYear(),
                            yearsOfHavingDriverLicense: new Date().getFullYear() - new Date(data.BecoamingDriverDate.value).getFullYear(),
                            rentDuration: 1,
                            location: data.City.value + " " + data.Street.value + " " + data.StreetNumber.value,
                            currentlyRentedCount: 0,
                            overallRentedCount: 0
                        })
                    }).then(response2 => response2.json())
                        .then(data2 => {
                            if (this.isinlist(carDetailsID)) {
                                this.removeitem(carDetailsID);
                            }

                            this.additem(carDetailsID, data2);



                        });
                }
                
            
            }).catch(error => this.setState({ checkPrice: true }));


        //if (!!profileData.json) {
        //    const customer = profileData.json;
            //const price = await fetch(process.env.REACT_APP_API + '/CarApi/GetPrice', {
            //    method: 'Post',
            //    headers: {
            //        'Accept': 'application/json',
            //        'Content-Type': 'application/json'
            //    },
            //    body: JSON.stringify({
            //        DateofBecomingDriver: customer.DateofBecomingDriver.value,
            //        Birthday: customer.Birthday.value,
            //        City: customer.City.value,
            //        Street: customer.Street.value,
            //        StreetNumber: customer.StreetNumber.value,
            //        NumberOfCurrentlyRentedCars: customer.NumberOfCurrentlyRentedCars.value,
            //        NumberOfOverallRentedCars: customer.NumberOfOverallRentedCars.value,
            //        CarDetalisID: this.props.cardetalisid
            //    })
            //});
            //
            //if (!!price.json) {
            //    alert("Error while accessing price data");
            //} else {
            //    if (this.PriceShower.isinlist(carDetailsID)) {
            //        this.PriceShower.removeitem(carDetailsID);
            //    }
            //
            //    this.additem(carDetailsID, price);
            //}

        //}
        //return true;
    }

    returnID(cardet) {
    if (this.props.api === "OurApi")
        return cardet;
    else if (this.props.api === "Alien")
        return this.takeQID(cardet);
    else
        return -1;
}


    render(){
        if(this.props.show)
        {
            const {details}=this.state;

            let ModalCheckPriceClose=()=>this.setState({checkPrice:false});
            let ModalRentClose=()=>this.setState({Rent:false});
 
            return(
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
                        {details.map(detal=>
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
                                            this.show_price(detal.CarDetailsID)!=null && 
                                            <Button className="mr-2" variant="success" 
                                                onClick={()=> this.setState({Rent:detal.CarDetailsID})}> 
                                                Wynajmij mnie
                                            </Button>
                                        }
                                        

                                        <CheckPriceForm isOpen={this.state.checkPrice}
                                            onHide={ModalCheckPriceClose}
                                            cardetalisid={detal.CarDetailsID}
                                            isinlist={this.isinlist}
                                            removeitem={this.removeItem}
                                            additem={this.additem}
                                            api={this.props.api }/>
                                        
                                        <RentMeForm isOpen={this.state.Rent}
                                            onHide={ModalRentClose}
                                            carDetailsID={this.returnID(detal.CarDetailsID)}
                                            api={this.props.api}/>
                                    </ButtonToolbar>
                                </td>
                            </tr>
                        ) }
                    </tbody>
                    </Table>
                
            )
        }
        else
            return(null)
    }
}

