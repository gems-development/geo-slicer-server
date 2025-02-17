import React from "react";
import "./MainPage.css"
import Map from "../components/Map";

function MainPage() {

    let map = <Map />;
    let geojson1 = {
        "type": "Polygon",
        "coordinates": [[[0, 0], [0, 1], [1, 1], [1, 0], [0, 0]]]
    }
    let geojson2 = {
        "type": "Polygon",
        "coordinates": [[[2, 0], [2, 1], [3, 1], [3, 0], [2, 0]]]
    }

    //map.setData(geojson1);

    return (
    <div>
        <Map/>
    </div>
    )
}

export default MainPage