import { Map } from 'maplibre-gl';
import 'maplibre-gl/dist/maplibre-gl.css';
import './Map.css';
import React, { useRef, useEffect } from 'react';
import styles from '../layersStyles';





export default function MyMap() {


    const geojson = useRef(null);

    const mapContainer = useRef(null);
    const map = useRef(null);
    const renderScreenRequest = useRef(null);
    const lng = 0;
    const lat = 0;
    const zoom = 5;

    function renderScreen() {
        let bounds = map.current.getBounds();
        let east = bounds.getEast();
        let north = bounds.getNorth();
        let west = bounds.getWest();
        let south = bounds.getSouth();


        if (renderScreenRequest.current != null) {
            renderScreenRequest.current.abort();
        }
        renderScreenRequest.current = new XMLHttpRequest();
        let xhr = renderScreenRequest.current;
        xhr.open("POST", "http://localhost:5148/geometry/byRectangle", true);
        xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
        xhr.send(JSON.stringify([{ "x": west, "y": south }, { "x": east, "y": north }]));
        xhr.onload = function () {
            console.log(xhr.response);
            let response = JSON.parse(xhr.response);
            let objectsByStyles = getObjectsBySlyles(response);

            for (const name in styles) {
                map.current.getSource(name).setData(objectsByStyles[name]);
            }
        };

    }

    function getObjectsBySlyles(layers) {
        let res = {};
        for (const style in styles) {
            res[style] = [];
        }

        for (const layer of layers) {
            if (styles[layer.Alias] === undefined) {
                layer.Alias = "other";
            }
            res[layer.Alias].push({ "type": "Feature", "geometry": layer.Result });
        }

        for (const style in styles) {
            res[style] = { "type": "FeatureCollection", "features": res[style] }
        }

        return res;
    }

    function showInfo(e) {
        let x = e.lngLat.lng;
        let y = e.lngLat.lat;

        let xhr = new XMLHttpRequest();
        xhr.open("POST", "http://localhost:5148/geometry/info/byClick", false);
        xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
        xhr.send(JSON.stringify({ "x": x, "y": y }));
        alert(xhr.response.replace("},{", "},\n{"));
    }

    useEffect(() => {
        if (map.current) return; // stops map from intializing more than once

        geojson.current = {
            "type": "Point",
            "coordinates": [0, 0]
        }

        map.current = new Map({
            container: mapContainer.current,
            center: [lng, lat],
            zoom: zoom
        });

        for (const layer in styles) {
            map.current.addSource(layer, {
                type: 'geojson',
                data: geojson.current
            });
            map.current.addLayer({
                'id': layer,
                'type': 'fill',
                'source': layer,
                'layout': {},
                'paint': styles[layer]
            });
        }


        map.current.on('moveend', () => { renderScreen(); });
        map.current.on('click', (e) => { showInfo(e); })

        renderScreen();

    }, [geojson]);



    return (
        <div ref={mapContainer} className="map" />
    );
}
