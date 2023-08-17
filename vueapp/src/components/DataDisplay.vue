<script setup lang="ts">
import { computed } from 'vue';
import ListData from './ListData.vue'
import { JsonifyProperty, PropertyTypes } from '../lib/typeDefinitions'
import 'element-plus/es/components/message/style/css'
import { Delete, MagicStick } from '@element-plus/icons-vue'

interface Props {
    jsonifyProperty: JsonifyProperty
}

const props = defineProps<Props>()
const computedColor = computed(() => {
switch (props.jsonifyProperty.depth){
    case 0: return '#4d908e';
    case 1: return '#43aa8b';
    case 2: return '#90be6d';
    case 3: return '#FACD62';
    case 4: return '#f8961e';
    case 5: return '#f3722c';
    case 6: return '#FB7E80';
    default: return '#277da1';
    }
})

function setToDefault(){
    if (props.jsonifyProperty.propertyType === PropertyTypes.List)
        props.jsonifyProperty.collections = props.jsonifyProperty.defaultValue

    else if (props.jsonifyProperty.propertyType === PropertyTypes.Object)
        props.jsonifyProperty.properties = props.jsonifyProperty.defaultValue

    else 
        props.jsonifyProperty.setValue = props.jsonifyProperty.defaultValue;
}

function setToNull(){
    if (props.jsonifyProperty.propertyType === PropertyTypes.List){
        props.jsonifyProperty.collections = null!
        props.jsonifyProperty.setValue = null
    }

    else if (props.jsonifyProperty.propertyType === PropertyTypes.Object)
        props.jsonifyProperty.properties = null!

    else 
        props.jsonifyProperty.setValue = null;
}
</script>

<style lang="scss">
.card-header {
  display: flex;
  justify-content: space-between;
  margin: 0;
  padding: 0;
}

.text {
  font-size: 18px;
  color: #1f363d;
  margin-right: 10px;
}

.propertyType {
    font-size: 16px;
    font-style: italic;
    font-weight: 800;
    color: #F2F7EF;
}

.buttonGroup {
    display: flex;
    justify-content: space-between;
}
</style>

<template>
    <div v-if="props.jsonifyProperty?.displayName">
        <el-card class="box-card" :style="{'background-color': computedColor}">
            <template #header>
                <div class="card-header">
                    <div>
                        <el-text class="text"><b>{{ props.jsonifyProperty.displayName }}</b></el-text>
                        <el-button :icon="MagicStick" color="black" circle @click="setToDefault"/>
                        <el-button :icon="Delete" color="black" circle @click="setToNull"/>
                    </div>
                    <el-text class="propertyType"> {{ props.jsonifyProperty.propertyType }}</el-text>
                    <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.List">
                        <el-input-number  v-model="props.jsonifyProperty.arraySize" />
                    </div>
                </div>
            </template>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Boolean" align="left"> 
                <el-switch 
                v-model="props.jsonifyProperty.setValue"
                style="--el-switch-on-color: #41F9F6; --el-switch-off-color: #f94144"
                
                active-text="True"
                inactive-text="False"
                />
            </div>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.DateTime" align="left"> 
                <div class="block">
                    <el-date-picker
                        v-model="props.jsonifyProperty.setValue"
                        type="datetime"
                        placeholder="Select date and time"
                    />
                </div>
            </div>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Decimal"> 
                    <el-input v-model="props.jsonifyProperty.setValue"  />
            </div>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Enum" align="left"> 
                <el-select v-model="props.jsonifyProperty.setValue" filterable clearable class="m-2" placeholder="Choose One" >
                <el-option v-for="(enumValue, index) in props.jsonifyProperty.enumeratedProperties" 
                            :key="index"
                            :label="enumValue"
                            :value="enumValue" />
                </el-select> 
            </div>
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Integer"> 
                <el-input v-model="props.jsonifyProperty.setValue"  />
            </div>

            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.List">
                <ListData :jsonifyProperty="props.jsonifyProperty" />
            </div>
            
            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.Object"> 
                <div v-for="(classProperty, index) in props.jsonifyProperty.properties" :key="index">
                    <DataDisplay :jsonifyProperty="classProperty"/>
                </div>
            </div>

            <div v-if="props.jsonifyProperty.propertyType === PropertyTypes.String"> 
                <el-input 
                v-model="props.jsonifyProperty.setValue" 
                placeholder="placeholder" 
                 
                clearable
                @clear="setToNull" />
            </div>
        </el-card>
    </div>
</template>