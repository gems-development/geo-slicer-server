import { GeoJSON } from "react-leaflet";
import { Component } from "react";

export default class MyGeoJson extends Component {
    constructor(props) {
        super(props);
        this.state = {
            geojson: {
                "type": "Polygon",
                "coordinates": [[[2, 0], [2, 1], [3, 1], [2, 0]]]
            }
        };
    }

    componentDidMount() {
        console.log("A");
        this.setState({
            geojson: {
                "type": "Polygon",
                "coordinates": [[[2, 0], [2, 1], [3, 1], [3, 0], [2, 0]]]
            }
        });
    }

    render() {
        console.log(this.state.geojson);
        return (
            <GeoJSON data={this.state.geojson} />
        )
    }
}