<script setup lang="ts">
import { defineProps, ref, computed } from 'vue';
import Jsonify from './Jsonify.vue'
import DataDisplay from './DataDisplay.vue'
import type {Ref} from 'vue';
import { Property, JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'
import 'element-plus/es/components/message/style/css'

interface Props {
    property: Property
}
const props = defineProps<Props>()

function getObject(properties: Property[]): Record<string, any> {
    let newObject: Record<string, any> = {}
    properties.forEach(property => {
        newObject[property.displayName] = getDefaultValue(property)
    })

    return newObject
}

function getDefaultValue(property: Property): any {
    switch(property.propertyType){
        case PropertyTypes.Boolean:
            return false;

        case PropertyTypes.DateTime:
            return new Date().toISOString();

        case PropertyTypes.Decimal:
            return 0.0;

        case PropertyTypes.Enum:{
            if (!property.enumeratedProperties){
                return null;
            }

            return property.enumeratedProperties[0]
        }

        case PropertyTypes.Integer:
            return 0;

        case PropertyTypes.List:
            let newArray = [];
            newArray.push(getObject(property.properties));
            return newArray;

        case PropertyTypes.Object:
            return getObject(property.properties)

        case PropertyTypes.String:
            return "abc";

        default:
            return "abc";
    }
}

function mapToJsonifyProperty(property: Property): JsonifyProperty {
    return {
        displayName: property.displayName,
        depth: property.depth,
        enumeratedProperties: property.enumeratedProperties,
        nullable: property.nullable,
        properties: mapToJsonifyProperties(property.properties),
        propertyType: property.propertyType,
        setValue: getDefaultValue(property),
        arraySize: 1
    }
}

function mapToJsonifyProperties(properties: Property[]): JsonifyProperty[] | [] {
    if (!properties){
        return []
    }

    return properties.map(property => mapToJsonifyProperty(property))
}

const mappedProperty = ref(mapToJsonifyProperty(props.property))

</script>

<template>
    <div v-if="mappedProperty">
        <el-row>
            <el-col>
                <p>Data Master</p>
                <el-row>
                    <DataDisplay :jsonify-property="mappedProperty" />
                </el-row>
            </el-col>
            <el-col>
                <p>Jsonify</p>
            </el-col>
            <el-row>
                <Jsonify :jsonify-property="mappedProperty" />
            </el-row>
        </el-row>
    </div>
</template>
