<script setup lang="ts">
import { defineProps, ref } from 'vue';
import type {Ref} from 'vue';
import { Property, PropertyTypes } from '../lib/typeDefinitions'

interface Props {
    classData: Property
}
const props = defineProps<Props>()
const currentProperty: Ref<string | number> = ref(props.classData.displayName)
    const testData = "hello";
    const testForm = "";

    if(props.classData.type === "String"){
        console.log("STRING")
    }

</script>

<template>
        <div v-if="props.classData">
            <el-row>
                <el-col>
                    <el-text class="mx-1" size="large">{{ props.classData.displayName }}</el-text>
                </el-col>
            </el-row>
            <el-row v-if="props.classData.propertyType === PropertyTypes.Boolean"> 
                <el-col>
                    <el-switch 
                    v-model="currentProperty"
                    size="small"
                    active-text="True"
                    inactive-text="False"
                    />
                </el-col>
            </el-row>
            <el-row v-if="props.classData.propertyType === PropertyTypes.Decimal"> 
                <el-col>
                    <el-input v-model="currentProperty" />
                </el-col>
            </el-row>
            <el-row v-if="props.classData.propertyType === PropertyTypes.Enum"> 
                <el-col>
                    <el-select v-model="currentProperty" filterable clearable class="m-2" placeholder="Choose One" size="large">
                    <el-option v-for="(enumValue, index) in props.classData.enumeratedProperties"
                                :key="index"
                                :label="enumValue"
                                :value="enumValue" />
                    </el-select> 
                </el-col>
            </el-row>
            <el-row v-if="props.classData.propertyType === PropertyTypes.Integer"> 
                <el-col>
                    <el-input v-model="currentProperty" />
                </el-col>
            </el-row>
            
            <el-row v-if="props.classData.propertyType === PropertyTypes.ObjectType"> 
                <el-col>
                    <el-row v-for="(classProperty, index) in props.classData.properties" :key="index">
                        <DataDisplay :classData="classProperty" />
                    </el-row>
                </el-col>
            </el-row>

            <el-row v-if="props.classData.propertyType === PropertyTypes.StringType"> 
                <el-col>
                    <el-form :model="testForm">
                        <el-form-item label="String Test">
                            <el-input v-model="testData" placeholder="placeholder" size="small"/>
                        </el-form-item>
                    </el-form>
                </el-col>
            </el-row>
        </div>
</template>