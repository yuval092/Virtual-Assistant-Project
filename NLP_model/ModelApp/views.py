from django.http import HttpResponse, JsonResponse, HttpRequest
from django.views.decorators.csrf import csrf_exempt
from ModelApp.BERT_Model import get_request_results
from django.shortcuts import render
from .apps import ModelappConfig
import threading
import time
import json
import sys
import os

# server_mutex = threading.Lock()

def _get_intent_and_slots(args):
    try:
        request = args['request']
        request.replace('\n', '')
    except:
        return JsonResponse({"Error": "Invalid Arguments"})

    try:
        # ModelappConfig.model_mutex.acquire()        # if acquired, the model is not 

        with open(ModelappConfig.input_path, 'w') as f:             # write request to input file
            f.write(request)
        
        while os.stat(ModelappConfig.output_path).st_size == 0:     # as long as no result returned
            time.sleep(0.1)
        
        try:
            with open(ModelappConfig.output_path, 'r') as f:            # read response from output file
                result = json.loads(f.readline())
            with open(ModelappConfig.output_path, 'w') as f:
                f.write("")
        except:
            result = {"CriticalError": "Model response was invalid"}

    except Exception as e:
        return JsonResponse({"CriticalError": e.args})

    return JsonResponse(result)


@csrf_exempt
def exec(request):
    if request.method == 'GET':
        return _get_intent_and_slots(request.GET)
    else:
        return HttpResponse("Bad request", 400)