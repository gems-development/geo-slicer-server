import { Map } from 'maplibre-gl';
import 'maplibre-gl/dist/maplibre-gl.css';
import './Map.css';
import React, { useRef, useEffect } from 'react';
import { styles, specialStyles } from '../layersStyles';





export default function MyMap() {


    const geojson = useRef(null);
    const selectedIds = useRef(null);

    const mapContainer = useRef(null);
    const map = useRef(null);
    const renderScreenRequest = useRef(null);
    const lng = 0;
    const lat = 0;
    const zoom = 15;

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
        xhr.open("POST", "http://localhost:5049/geometry/byRectangle", true);
        xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
        xhr.send(JSON.stringify([{ "x": west, "y": south }, { "x": east, "y": north }]));
        xhr.onload = function () {
            console.log(xhr.response);
            let response = JSON.parse(xhr.response);

            geojson.current = response;

            let objectsByStyles = getObjectsBySlyles(response);

            for (const name in styles) {
                map.current.getSource(name).setData(objectsByStyles[name]);
            }
            highlight(selectedIds.current);
        };

    }

    function getObjectsBySlyles(geoms) {
        let res = {};
        for (const style in styles) {
            res[style] = [];
        }

        for (const geom of geoms) {
            if (styles[geom.layerAlias] === undefined) {
                geom.layerAlias = "DefaultLayer";
            }
            res[geom.layerAlias].push({ "type": "Feature", "geometry": geom.result });
        }

        for (const style in styles) {
            res[style] = { "type": "FeatureCollection", "features": res[style] }
        }

        return res;
    }

    function highlight(ids) {
        let highlighted = [];

        for(const geom of geojson.current){
            if(ids.includes(geom.id)) {
                highlighted.push({ "type": "Feature", "geometry": geom.result })
            }
        }

        map.current.getSource("highlighted").setData({ "type": "FeatureCollection", "features": highlighted });
    }

    function showInfo(e) {
        let x = e.lngLat.lng;
        let y = e.lngLat.lat;

        let xhr = new XMLHttpRequest();
        xhr.open("POST", "http://localhost:5049/geometry/byClick", true);
        xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
        xhr.send(JSON.stringify({ "x": x, "y": y }));
        xhr.onload = function () {
            alert(xhr.response.replace("},{", "},\n{"));

            let response = JSON.parse(xhr.response);

            selectedIds.current = response.map(layer => layer.id);
            highlight(selectedIds.current);
        }
    }

    useEffect(() => {
        if (map.current) return; // stops map from intializing more than once

        geojson.current = {
            "type": "Point",
            "coordinates": [0, 0]
        };
        selectedIds.current = [];

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
        
        for (const layer in specialStyles) {
            map.current.addSource(layer, {
                type: 'geojson',
                data: geojson.current
            });
            map.current.addLayer({
                'id': layer,
                'type': 'fill',
                'source': layer,
                'layout': {},
                'paint': specialStyles[layer]
            });
        }


        map.current.on('moveend', () => { renderScreen(); });
        //map.current.on('move', () => { renderScreen(); });
        map.current.on('click', (e) => { showInfo(e); })

        renderScreen();

    }, [geojson, selectedIds]);



    return (
        <div ref={mapContainer} className="map" />
    );
}
